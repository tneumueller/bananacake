using System;
using System.Linq;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    public class OperatorInvoke : Operator, IRValue {
        public OperatorInvoke() {
            System.Console.WriteLine("New OperatorInvoke");
        }

        public static OperatorInvoke Parse(Scopes.Scope scope, Expression functionNode, Token[] argList) {
            var op = new OperatorInvoke();

            var arguments = Nodes.Functions.ArgumentsNode.Parse(functionNode, scope, argList);
            op.Left = functionNode;
            op.Right = new Expression(
                argList.FirstOrDefault() ?? functionNode.DefiningToken,
                scope,
                arguments
            );

            var function = (functionNode.Root as SymbolNode)?.Symbol as Types.FunctionType;
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