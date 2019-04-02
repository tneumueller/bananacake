using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators {
    public class RuntimeOperatorAccess : RuntimeOperator {
        public RuntimeOperatorAccess(OperatorAccess op, RuntimeScope scope) : base(op, scope) {}

        public override RuntimeValueNode Evaluate() {
            var left = new RuntimeExpression(Operator.Left, RuntimeScope).Evaluate();
            var right = new RuntimeExpression(
                Operator.Right,
                RuntimeScope.ResolveRuntimeScope(Operator.Left.ReturnType.Scope)
            ).Evaluate();
            return null;
        }
    }
}