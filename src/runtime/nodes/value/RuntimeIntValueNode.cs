using BCake.Parser.Syntax.Expressions.Nodes.Value;

namespace BCake.Runtime.Nodes.Value {
    [RuntimeValueNode(
        Value = 0,
        ValueNodeType = typeof(IntValueNode)
    )]
    public class RuntimeIntValueNode : RuntimeValueNode {
        public RuntimeIntValueNode(IntValueNode valueNode, RuntimeScope scope)
            : base(valueNode, IntValueNode.Type, scope) {}

        public override RuntimeValueNode OpPlus(RuntimeValueNode other) {
            return Wrap((int)Value + (int)other.Value);
        }
        public override RuntimeValueNode OpMinus(RuntimeValueNode other) {
            return Wrap((int)Value - (int)other.Value);
        }
        public override RuntimeValueNode OpMultiply(RuntimeValueNode other) {
            return Wrap((int)Value * (int)other.Value);
        }
        public override RuntimeValueNode OpDivide(RuntimeValueNode other) {
            return Wrap((int)Value / (int)other.Value);
        }

        public override RuntimeValueNode OpGreater(RuntimeValueNode other) {
            return Wrap((int)Value > (int)other.Value);
        }
        public override RuntimeValueNode OpEqual(RuntimeValueNode other) {
            return Wrap((int)Value == (int)other.Value);
        }
        public override RuntimeValueNode OpSmaller(RuntimeValueNode other) {
            return Wrap((int)Value < (int)other.Value);
        }

        private RuntimeIntValueNode Wrap(int value) {
            return new RuntimeIntValueNode(
                new IntValueNode(
                    DefiningToken,
                    value
                ),
                RuntimeScope
            );
        }

        private RuntimeBoolValueNode Wrap(bool value) {
            return new RuntimeBoolValueNode(
                new BoolValueNode(
                    DefiningToken,
                    value
                ),
                RuntimeScope
            );
        }
    }
}