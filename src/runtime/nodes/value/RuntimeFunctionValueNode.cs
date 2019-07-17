using BCake.Parser.Syntax.Types;

namespace BCake.Runtime.Nodes.Value {
    public class RuntimeFunctionValueNode : RuntimeValueNode {
        public RuntimeFunctionValueNode(FunctionType function, RuntimeScope scope)
            : base(null, function, scope) {
            Value = function;
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