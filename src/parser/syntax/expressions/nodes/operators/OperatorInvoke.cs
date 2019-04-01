using System;
using System.Linq;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    [Operator(
        Symbol = "(",
        CheckReturnTypes = false
    )]
    public class OperatorInvoke : Operator, IRValue {
        private Expression _functionNode;
        private Types.FunctionType _function;

        public OperatorInvoke() {
            System.Console.WriteLine("New OperatorInvoke");
        }

        protected override Expression ParseLeft(Scopes.Scope scope, Token[] tokens, Scopes.Scope typeSource) {
            // tokens must have length 1 because we check in the ParseHandler function
            var t0 = tokens[0];

            Types.Type symbol;
            var _symbol = SymbolNode.GetSymbol(scope, t0);
            if (_symbol == null) throw new Exceptions.UndefinedSymbolException(t0, t0.Value, scope);

            if (_symbol is Types.CompositeType) symbol = (_symbol as Types.CompositeType).OperatorAccess.ReturnType;
            else symbol = _symbol;

            if (!(symbol is Types.FunctionType || symbol is Types.ClassType)) throw new Exception("TODO invalid call");

            _functionNode = Expression.Parse(scope, tokens.Take(1).ToArray());
            var unresolvedFunction = (_functionNode.Root as SymbolNode)?.Symbol;
            _function = Types.CompositeType.Resolve<Types.FunctionType>(unresolvedFunction);

            /* if the function is null, this must be a constructor call */
            if (_function == null) {
                var classType = Types.CompositeType.Resolve((_functionNode.Root as SymbolNode)?.Symbol);

                _function = classType?.Scope.GetSymbol("!constructor", true) as Types.FunctionType;
                if (_function.Access != "public" && !scope.IsChildOf(classType.Scope)) {
                    throw new Exceptions.AccessViolationException(_functionNode.DefiningToken, _function, scope);
                }

                unresolvedFunction = _function;
            }

            _functionNode = new Expression(
                _functionNode.DefiningToken,
                _functionNode.Scope,
                new SymbolNode(_functionNode.DefiningToken, unresolvedFunction)
            );
            return _functionNode;
        }

        protected override Expression ParseRight(Scopes.Scope scope, Token[] tokens, Scopes.Scope typeSource) {
            var argListClose = Parser.findClosingScope(
                tokens.Prepend(new Token { Value = "(" }).ToArray(),
                0
            ) - 1;
            var argList = tokens.Take(argListClose).ToArray();

            var arguments = Nodes.Functions.ArgumentsNode.Parse(_functionNode, scope, argList);
            if (arguments.Arguments.Length != _function.Parameters.Length)
            throw new Exceptions.InvalidArgumentsException(argList[0], _function, arguments.Arguments);

            for (int i = 0; i < _function.Parameters.Length; ++i) {
                var paramType = _function.Parameters[i].Type;
                var argType = arguments.Arguments[i].Expression.Root.ReturnType;
                if (paramType != argType) throw new Exceptions.InvalidArgumentsException(argList[0], _function, arguments.Arguments);
            }

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

            var t0 = tokens[0];
            var t1 = tokens[1];

            if (!SymbolNode.CouldBeIdentifier(t0.Value, out var m0) || t1.Value != "(") return false;

            return true;
        }
    }
}