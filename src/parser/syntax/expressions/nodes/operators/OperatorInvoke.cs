using System;
using System.Linq;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    public class OperatorInvoke : Operator, IRValue {
        public OperatorInvoke() {
            System.Console.WriteLine("New OperatorInvoke");
        }

        public static OperatorInvoke Parse(Scopes.Scope scope, Expression functionNode, Token[] argList) {
            var op = new OperatorInvoke();

            Types.FunctionType function;
            var _function = (functionNode.Root as SymbolNode)?.Symbol;
            if (_function is Types.CompositeType) function = (_function as Types.CompositeType).OperatorAccess.ReturnType as Types.FunctionType;
            else function = _function as Types.FunctionType;

            /* if the function is null, this must be a constructor call */
            if (function == null) {
                Types.Type classType;
                var _classType = (functionNode.Root as SymbolNode)?.Symbol;

                if (_classType is Types.CompositeType) classType = (_classType as Types.CompositeType).OperatorAccess.ReturnType;
                else classType = _classType;

                function = classType?.Scope.GetSymbol("!constructor", true) as Types.FunctionType;
                if (function.Access != "public" && !scope.IsChildOf(classType.Scope)) {
                    throw new Exceptions.AccessViolationException(functionNode.DefiningToken, function, scope);
                }
            }

            functionNode = new Expression(
                functionNode.DefiningToken,
                functionNode.Scope,
                new SymbolNode(functionNode.DefiningToken, function)
            );
            op.Left = functionNode;

            var arguments = Nodes.Functions.ArgumentsNode.Parse(functionNode, scope, argList);
            op.Right = new Expression(
                argList.FirstOrDefault() ?? functionNode.DefiningToken,
                scope,
                arguments
            );

            if (arguments.Arguments.Length != function.Parameters.Length)
            throw new Exceptions.InvalidArgumentsException(argList[0], function, arguments.Arguments);

            for (int i = 0; i < function.Parameters.Length; ++i) {
                var paramType = function.Parameters[i].Type;
                var argType = arguments.Arguments[i].Expression.Root.ReturnType;
                if (paramType != argType) throw new Exceptions.InvalidArgumentsException(argList[0], function, arguments.Arguments);
            }

            return op;
        }
    }
}