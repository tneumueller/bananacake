using BCake.Parser.Syntax.Scopes;
using BCake.Parser.Syntax.Types;
using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Value;

namespace BCake.Runtime.Nodes.Value {
    // [RuntimeValueNode(
    //     Value = false,
    //     ValueNodeType = typeof()
    // )]
    public class RuntimeClassInstanceValueNode : RuntimeValueNode, IAccessible {
        public RuntimeClassInstanceValueNode(Node node, ComplexType type, RuntimeScope scope)
            : base(node, ConstructScope(type, scope)) {
            Value = this;

            RuntimeScope.SetValue("this", this);
            foreach (var m in RuntimeScope.Scope.AllMembers) {
                if (!(m.Value is FunctionType)) continue;

                RuntimeScope.SetValue(
                    m.Key,
                    new RuntimeFunctionValueNode(
                        m.Value as FunctionType,
                        new RuntimeScope(
                            RuntimeScope,
                            m.Value.Scope
                        )
                    )
                );
            }
        }

        protected static RuntimeScope ConstructScope(ComplexType type, RuntimeScope parent) {
            return new RuntimeScope(
                parent,
                type.Scope
            );
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