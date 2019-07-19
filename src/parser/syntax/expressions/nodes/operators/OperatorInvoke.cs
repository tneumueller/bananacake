using System;
using System.Linq;
using BCake.Parser.Syntax.Expressions.Nodes.Functions;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    [Operator(
        Symbol = "(",
        CheckReturnTypes = false,
        Direction = OperatorAttribute.EvaluationDirection.RightToLeft
    )]
    public class OperatorInvoke : Operator, IRValue {
        public Types.FunctionType Function { get; protected set; }
        protected Expression _functionNode;

        public OperatorInvoke() {}

        protected override Expression ParseLeft(Scopes.Scope scope, Token[] tokens, Scopes.Scope typeSource) {
            Types.Type symbol;

            _functionNode = Expression.Parse(scope, tokens, typeSource);

            switch (_functionNode.Root) {
                case SymbolNode s: symbol = s.Symbol; break;
                case OperatorAccess o: symbol = o.ReturnSymbol; break;
                default: symbol = _functionNode.ReturnType; break;
            }

            if (!(symbol is Types.FunctionType || symbol is Types.ClassType)) throw new Exception("TODO invalid call");
            Function = symbol as Types.FunctionType;

            /* if the function is null, this must be a constructor call */
            if (Function == null) {
                var classType = symbol as Types.ClassType;

                Function = classType?.Scope.GetSymbol("!constructor", true) as Types.FunctionType;
                if (Function.Access != Types.Access.@public && !scope.IsChildOf(classType.Scope)) {
                    throw new Exceptions.AccessViolationException(_functionNode.DefiningToken, Function, scope);
                }

                symbol = Function;
            }

            return _functionNode;
        }

        protected override Expression ParseRight(Scopes.Scope scope, Token[] tokens, Scopes.Scope typeSource) {
            var argListClose = Parser.findClosingScope(
                tokens.Prepend(new Token { Value = "(" }).ToArray(),
                0
            ) - 1;
            var argList = tokens.Take(argListClose).ToArray();

            var arguments = Nodes.Functions.ArgumentsNode.Parse(_functionNode, scope, argList);
            var overloads = Function.Overloads.Prepend(Function);
            Types.FunctionType overload = null;
            foreach (var func in overloads) {
                if (!func.ParameterListDiffers(arguments.Arguments.Select(a => a.Expression.ReturnType))) {
                    overload = func;
                    break;
                }
            }

            if (overload == null) {
                if (argList.Length > 0) throw new Exceptions.InvalidArgumentsException(argList.FirstOrDefault(), Function, arguments.Arguments);
                else throw new Exceptions.InvalidArgumentsException(tokens[0], Function, arguments.Arguments);
            }

            Function = overload;

            return new Expression(
                argList.FirstOrDefault() ?? _functionNode.DefiningToken,
                scope,
                arguments
            );
        }

        [OperatorParsePreflight]
        public static bool ParsePreflight(OperatorParseInfo info) {
            var tokens = info.Tokens;

            if (tokens.Length <= 2) return false;

            var left = string.Join("", info.TokensLeft.Select(t => t.Value));
            if (!SymbolNode.CouldBeIdentifier(left, out var m)) {
                return false;
            }

            return true;
        }

        public static OperatorInvoke FromOverloadableOperator(
            Scopes.Scope scope,
            OverloadableOperator op,
            Types.FunctionType operatorFunction,
            Expression operatorFunctionExpression
        ) {
            var opInvoke = new OperatorInvoke();
            opInvoke.Function = operatorFunction;

            // opInvoke.Left = new Expression(
            //     op.DefiningToken,
            //     scope,
            //     new ValueN
            // );

            opInvoke.Left = new Expression(
                op.DefiningToken,
                operatorFunctionExpression.ReturnType.Scope,
                new SymbolNode(
                    op.DefiningToken,
                    operatorFunction
                )
            );
            opInvoke.Right = new Expression(
                op.DefiningToken,
                scope,
                new ArgumentsNode(
                    op.DefiningToken,
                    null,
                    new ArgumentsNode.Argument[] {
                        new ArgumentsNode.Argument(op.Left, true), // provide the left hand side for native implementations
                        new ArgumentsNode.Argument(op.Right)
                    }
                )
            );

            return opInvoke;
        }
    }
}