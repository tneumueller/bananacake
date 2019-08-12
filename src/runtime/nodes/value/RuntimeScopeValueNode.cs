using BCake.Parser.Syntax.Scopes;
using BCake.Parser.Syntax.Types;
using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Value;

namespace BCake.Runtime.Nodes.Value {
    // [RuntimeValueNode(
    //     Value = false,
    //     ValueNodeType = typeof()
    // )]
    public class RuntimeTypeValueNode : RuntimeValueNode, IAccessible {
        public RuntimeTypeValueNode(Type type) : base(null, type, RuntimeScope.ResolveRuntimeScope(type.Scope)) {
            foreach (var m in RuntimeScope.Scope.AllMembers) {
                if (!(m.Value is FunctionType)) continue;

                RuntimeScope.SetValue(
                    m.Key,
                    new RuntimeFunctionValueNode(
                        m.Value as FunctionType,
                        RuntimeScope
                    )
                );
            }
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