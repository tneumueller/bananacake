using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Parser.Syntax.Expressions.Nodes.Operators.Logical;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators.Logical {
    public class RuntimeOperatorLogicalOr : RuntimeOperator {
        public RuntimeOperatorLogicalOr(OperatorLogicalOr op, RuntimeScope scope) : base(op, scope) {}

        public override RuntimeValueNode Evaluate() {
            var l = new RuntimeExpression(
                Operator.Left,
                RuntimeScope
            ).Evaluate();
            
            var res = l.OpEqual(WrapBool(true, l));
            if ((bool)res.Value) return WrapBool(true, res);

            var r = new RuntimeExpression(
                Operator.Right,
                RuntimeScope
            ).Evaluate();

            res = r.OpEqual(WrapBool(true, r));
            if ((bool)res.Value) return WrapBool(true, res);

            return WrapBool(false, r);
        }

        private RuntimeBoolValueNode WrapBool(bool b, RuntimeValueNode referenceNode) {
            return new RuntimeBoolValueNode(
                new Parser.Syntax.Expressions.Nodes.Value.BoolValueNode(
                    referenceNode.DefiningToken,
                    b
                ),
                referenceNode.RuntimeScope
            );
        }
    }
}