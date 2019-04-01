using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators {
    public class RuntimeOperatorNew : RuntimeOperator {
        public RuntimeOperatorNew(OperatorNew op, RuntimeScope scope) : base(op, scope) {}

        public override RuntimeValueNode Evaluate() {
            return new RuntimeExpression(
                Operator.Right,
                RuntimeScope
            ).Evaluate();
        }
    }
}