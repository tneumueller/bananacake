using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators {
    public class RuntimeOperatorAccess : RuntimeOperator {
        public RuntimeOperatorAccess(OperatorAccess op, RuntimeScope scope) : base(op, scope) { }

        public override RuntimeValueNode Evaluate() {
            var left = new RuntimeExpression(Operator.Left, RuntimeScope).Evaluate();
            if (left == null) {
                throw new Exceptions.NullReferenceException((Operator.Left.Root as SymbolNode)?.Symbol, DefiningToken);
            }
            if (!(left is IAccessible)) {
                throw new Exceptions.RuntimeException($"Symbol is not accessible", left.DefiningToken);
            }

            var leftAccessible = left as IAccessible;
            return leftAccessible.AccessMember(
                (Operator as OperatorAccess).MemberToAccess.Name
            );
        }
    }
}