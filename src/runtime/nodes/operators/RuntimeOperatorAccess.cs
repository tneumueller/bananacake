using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators {
    public class RuntimeOperatorAccess : RuntimeOperator {
        public RuntimeOperatorAccess(OperatorAccess op, RuntimeScope scope) : base(op, scope) {}

        public override RuntimeValueNode Evaluate() {
            // todo
            return null;
        }
    }
}