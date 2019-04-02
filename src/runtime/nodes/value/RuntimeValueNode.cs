using System.Linq;
using BCake.Parser.Syntax.Expressions;
using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Value;
using BCake.Parser.Syntax.Types;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Value {
    public abstract class RuntimeValueNode : RuntimeNode {
        public object Value { get; protected set; }
        public RuntimeValueNode(Node node, RuntimeScope scope) : base(node.DefiningToken, scope) {
            if (node is ValueNode) Value = (node as ValueNode).Value;
        }

        public new static RuntimeValueNode Create(Node node, RuntimeScope scope) {
            switch (node) {
                case IntValueNode i: return new RuntimeIntValueNode(i, scope);
                case BoolValueNode b: return new RuntimeBoolValueNode(b, scope);
                case NullValueNode n: return new RuntimeNullValueNode(n, scope);
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

        protected static RuntimeValueNodeAttribute GetMetaInfo(Type t) {
            return t.GetType().GetCustomAttributes(
                typeof(RuntimeValueNodeAttribute),
                true
            ).FirstOrDefault() as RuntimeValueNodeAttribute;
        }

        public static RuntimeValueNode InstantiateWithDefaultValue(Type t, RuntimeScope scope) {
            var meta = RuntimeValueNode.GetMetaInfo(t);

            var constrRuntime = t.GetType().GetConstructor(
                new System.Type[] {
                    typeof(Node),
                    typeof(RuntimeScope)
                }
            );
            if (constrRuntime == null) {
                throw new System.Exception($"FATAL cannot create default instance of {t.GetType().FullName} because constructor with parameters (Node, RuntimeScope) does not exist");
            }

            var constrValue = meta.ValueNodeType.GetConstructors().FirstOrDefault();
            if (constrValue == null) {
                throw new System.Exception($"FATAL cannot create default ValueNode of {t.GetType().FullName} because constructor with parameters (Token, RuntimeScope) does not exist");
            }

            return constrRuntime.Invoke(new object[] {
                constrValue.Invoke(new object[] {
                    null,
                    meta.Value
                }),
                scope
            }) as RuntimeValueNode;
        }
    }
}