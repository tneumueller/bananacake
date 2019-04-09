using BCake.Parser.Syntax;
using BCake.Parser.Syntax.Types;

namespace BCake.Runtime.Nodes.Value {
    public class RuntimeNamespaceValueNode : RuntimeValueNode, IAccessible {
        public RuntimeNamespaceValueNode(Namespace ns, RuntimeScope scope) : base(null, scope) {
            Value = ns;
        }

        public RuntimeValueNode AccessMember(string name) {
            return RuntimeScope.GetValue(name);
        }

        public override RuntimeValueNode OpPlus(RuntimeValueNode other) {
            throw new Exceptions.RuntimeException("", DefiningToken);
        }
        public override RuntimeValueNode OpMinus(RuntimeValueNode other) {
            throw new Exceptions.RuntimeException("", DefiningToken);
        }
        public override RuntimeValueNode OpMultiply(RuntimeValueNode other) {
            throw new Exceptions.RuntimeException("", DefiningToken);
        }
        public override RuntimeValueNode OpDivide(RuntimeValueNode other) {
            throw new Exceptions.RuntimeException("", DefiningToken);
        }

        public override RuntimeValueNode OpGreater(RuntimeValueNode other) {
            throw new Exceptions.RuntimeException("", DefiningToken);
        }
        public override RuntimeValueNode OpEqual(RuntimeValueNode other) {
            throw new Exceptions.RuntimeException("", DefiningToken);
        }
        public override RuntimeValueNode OpSmaller(RuntimeValueNode other) {
            throw new Exceptions.RuntimeException("", DefiningToken);
        }
    }
}