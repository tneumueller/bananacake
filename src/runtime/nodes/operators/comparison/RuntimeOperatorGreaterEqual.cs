using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Parser.Syntax.Expressions.Nodes.Operators.Comparison;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators.Comparison {
    public class RuntimeOperatorGreaterEqual : RuntimeOperator {
        public RuntimeOperatorGreaterEqual(OperatorGreaterEqual op, RuntimeScope scope) : base(op, scope) {}

        public override RuntimeValueNode Evaluate() {
            var l = new RuntimeExpression(
                Operator.Left,
                RuntimeScope
            ).Evaluate();
            var r = new RuntimeExpression(
                Operator.Right,
                RuntimeScope
            ).Evaluate();

            RuntimeValueNode res;
            res = l.OpEqual(r);
            if ((bool)res.Value) return res;
            res = l.OpGreater(r);
            if ((bool)res.Value) return res;
            return res;
        }
    }
}