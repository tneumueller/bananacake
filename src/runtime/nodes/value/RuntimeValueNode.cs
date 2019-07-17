using System.Linq;
using BCake.Parser.Syntax.Expressions;
using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Value;
using BCake.Parser.Syntax.Types;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Value {
    public abstract class RuntimeValueNode : RuntimeNode {
        public Type Type { get; protected set; }
        public object Value { get; protected set; }
        public RuntimeValueNode(Node node, Type type, RuntimeScope scope)
            : base(node?.DefiningToken, ConstructScope(type, scope)) {
            if (node is ValueNode) Value = (node as ValueNode).Value;
            Type = type;
        }

        public new static RuntimeValueNode Create(Node node, RuntimeScope scope) {
            switch (node) {
                case IntValueNode i: return new RuntimeIntValueNode(i, scope);
                case BoolValueNode b: return new RuntimeBoolValueNode(b, scope);
                case NullValueNode n: return new RuntimeNullValueNode(n, scope);
                case StringValueNode s: return new RuntimeStringValueNode(s, scope);
                //case Type t: 
            }

            return null; // todo handle this case properly (exception?)
        }

        protected static RuntimeScope ConstructScope(Type type, RuntimeScope parent) {
            return new RuntimeScope(
                parent,
                type.Scope
            );
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