using System.Linq;
using System.Collections.Generic;
using BCake.Parser.Syntax.Scopes;
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

            // var functionNode = RuntimeScope.ResolveSymbolNode(Operator.Left.Root as SymbolNode);
            // var function = functionNode.Symbol as FunctionType;
            
            var function = (Operator as OperatorInvoke).Function;
            var functionNode = function.Root;

            var functionScope = RuntimeScope.ResolveRuntimeScope(function.Scope);
            RuntimeFunction runtimeFunction;

            if (function.Name == "!constructor") {
                var constructingType = function.Scope.GetClosestType();
                var typeInstance = new RuntimeClassInstanceValueNode(
                    functionNode,
                    constructingType,
                    RuntimeScope.ResolveRuntimeScope(constructingType.Scope)
                );

                var constr = typeInstance.AccessMember("!constructor");
                runtimeFunction = new RuntimeFunction(
                    function,
                    constr.RuntimeScope,
                    argumentsValues.ToArray()
                );

                runtimeFunction.Evaluate();
                return typeInstance;
            } else {
                var left = new RuntimeExpression(
                    Operator.Left,
                    RuntimeScope.ResolveRuntimeScope(Operator.Left.Scope)
                ).Evaluate();

                if (!(left is RuntimeFunctionValueNode)) throw new Exceptions.RuntimeException("Cannot invoke non-function", Operator.Left.DefiningToken);

                runtimeFunction = new RuntimeFunction(
                    left.Value as FunctionType,
                    left.RuntimeScope,
                    argumentsValues.ToArray()
                );
                return runtimeFunction.Evaluate();
            }
        }
    }
}