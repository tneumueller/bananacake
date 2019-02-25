using System;
using System.Linq;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    public class OperatorInvoke : Operator, IRValue {
        public OperatorInvoke() {
            System.Console.WriteLine("New OperatorInvoke");
        }

        public static OperatorInvoke Parse(Scopes.Scope scope, Expression functionNode, Token[] argList) {
            var op = new OperatorInvoke();

            var function = (functionNode.Root as SymbolNode)?.Symbol as Types.FunctionType;
            if (function == null) {
                var classType = (functionNode.Root as SymbolNode)?.Symbol as Types.ClassType;
                function = classType?.Scope.GetSymbol("!constructor", true) as Types.FunctionType;
            }

            functionNode = new Expression(
                functionNode.DefiningToken,
                functionNode.Scope,
                new SymbolNode(function)
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