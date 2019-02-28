using System.Linq;
using System.Collections.Generic;
using BCake.Parser.Syntax.Types;
using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Functions;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators {
    public class RuntimeOperatorInvoke : RuntimeOperator {
        public RuntimeOperatorInvoke(OperatorInvoke op, RuntimeScope scope) : base(op, scope) {}

        public override RuntimeValueNode Evaluate() {
            var argumentsNode = Operator.Right.Root as ArgumentsNode;
            var arguments = argumentsNode.Arguments;
            var argumentsValues = arguments
                .Select(arg => {
                    var exp = new RuntimeExpression(
                        arg.Expression,
                        RuntimeScope
                    );
                    return exp.Evaluate();
                })
                .Cast<RuntimeValueNode>();

            var functionNode = Operator.Left.Root as SymbolNode;
            var function = functionNode.Symbol as FunctionType;

            var functionScope = RuntimeScope.ResolveRuntimeScope(function.Scope);
            var runtimeFunction = new RuntimeFunction(function, new RuntimeScope(functionScope, function.Scope), argumentsValues.ToArray());

            return runtimeFunction.Evaluate();
        }
    }
}