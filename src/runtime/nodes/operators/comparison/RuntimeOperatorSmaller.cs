using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Parser.Syntax.Expressions.Nodes.Operators.Comparison;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators.Comparison {
    public class RuntimeOperatorSmaller : RuntimeOperator {
        public RuntimeOperatorSmaller(OperatorSmaller op, RuntimeScope scope) : base(op, scope) {}

        public override RuntimeValueNode Evaluate() {
            var l = new RuntimeExpression(
                Operator.Left,
                RuntimeScope
            ).Evaluate();
            var r = new RuntimeExpression(
                Operator.Right,
                RuntimeScope
            ).Evaluate();

            return l.OpSmaller(r);
        }
    }
}