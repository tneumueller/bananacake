using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators {
    public class RuntimeOperatorDivide : RuntimeOperator {
        public RuntimeOperatorDivide(OperatorDivide op, RuntimeScope scope) : base(op, scope) {}

        public override RuntimeValueNode Evaluate() {
            var l = new RuntimeExpression(
                Operator.Left,
                RuntimeScope
            ).Evaluate();
            var r = new RuntimeExpression(
                Operator.Right,
                RuntimeScope
            ).Evaluate();

            return l.OpDivide(r);
        }
    }
}