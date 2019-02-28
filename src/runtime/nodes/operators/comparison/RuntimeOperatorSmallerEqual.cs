using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Parser.Syntax.Expressions.Nodes.Operators.Comparison;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators.Comparison {
    public class RuntimeOperatorSmallerEqual : RuntimeOperator {
        public RuntimeOperatorSmallerEqual(OperatorSmallerEqual op, RuntimeScope scope) : base(op, scope) {}

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
            res = l.OpSmaller(r);
            if ((bool)res.Value) return res;
            return res;
        }
    }
}