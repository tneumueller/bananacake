using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Parser.Syntax.Expressions.Nodes.Operators.Comparison;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators.Comparison {
    public class RuntimeOperatorNotEqual : RuntimeOperator {
        public RuntimeOperatorNotEqual(OperatorNotEqual op, RuntimeScope scope) : base(op, scope) {}

        public override RuntimeValueNode Evaluate() {
            var l = new RuntimeExpression(
                Operator.Left,
                RuntimeScope
            ).Evaluate();
            var r = new RuntimeExpression(
                Operator.Right,
                RuntimeScope
            ).Evaluate();

            var res = l.OpEqual(r);
            return new RuntimeBoolValueNode(
                new Parser.Syntax.Expressions.Nodes.Value.BoolValueNode(
                    res.DefiningToken,
                    !(bool)res.Value
                ),
                res.RuntimeScope
            );
        }
    }
}