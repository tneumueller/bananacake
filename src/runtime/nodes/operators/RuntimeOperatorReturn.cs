using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Runtime.Nodes.Value;

namespace BCake.Runtime.Nodes.Operators {
    public class RuntimeOperatorReturn : RuntimeOperator {
        public RuntimeOperatorReturn(OperatorReturn op, RuntimeScope scope) : base(op, scope) {}

        public override RuntimeValueNode Evaluate() {
            return RuntimeNode.Create(
                Operator.Right.Root,
                RuntimeScope
            ).Evaluate();
        }
    }
}