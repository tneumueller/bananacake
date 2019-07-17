using BCake.Parser.Syntax.Types;
using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators {
    public class RuntimeOperatorCast : RuntimeOperator {
        public RuntimeOperatorCast(OperatorCast op, RuntimeScope scope) : base(op, scope) {}

        public override RuntimeValueNode Evaluate() {
            var left = new RuntimeExpression(
                Operator.Left,
                RuntimeScope
            ).Evaluate();

            var right = (Operator.Right.Root as SymbolNode).Symbol;
            var casterFunction = left.RuntimeScope.GetValue($"!as_{ right.Name }");

            var args = left.Type is PrimitiveType ? new RuntimeValueNode[] { left } : new RuntimeValueNode[] {};
            var runtimeCasterFunction = new RuntimeFunction(
                casterFunction.Value as FunctionType,
                left.RuntimeScope,
                args
            );
            return runtimeCasterFunction.Evaluate();
        }
    }
}