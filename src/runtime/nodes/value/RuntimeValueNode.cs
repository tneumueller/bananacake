using BCake.Parser.Syntax.Expressions;
using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Value;
using BCake.Parser.Syntax.Types;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Value {
    public abstract class RuntimeValueNode : RuntimeNode {
        public object Value { get; protected set; }
        public RuntimeValueNode(ValueNode node, RuntimeScope scope) : base(node.DefiningToken, scope) {
            Value = node.Value;
        }

        public new static RuntimeValueNode Create(Node node, RuntimeScope scope) {
            switch (node) {
                case IntValueNode i: return new RuntimeIntValueNode(i, scope);
                case BoolValueNode b: return new RuntimeBoolValueNode(b, scope);
                //case Type t: 
            }

            return null; // todo handle this case properly (exception?)
        }

        public override RuntimeValueNode Evaluate() {
            return this;
        }

        public abstract RuntimeValueNode OpPlus(RuntimeValueNode other);
        public abstract RuntimeValueNode OpMinus(RuntimeValueNode other);
        public abstract RuntimeValueNode OpMultiply(RuntimeValueNode other);
        public abstract RuntimeValueNode OpDivide(RuntimeValueNode other);

        public abstract RuntimeValueNode OpGreater(RuntimeValueNode other);
        public abstract RuntimeValueNode OpEqual(RuntimeValueNode other);
        public abstract RuntimeValueNode OpSmaller(RuntimeValueNode other);
    }
}