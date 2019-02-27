using BCake.Parser.Syntax.Expressions;
using BCake.Runtime.Nodes.Value;

namespace BCake.Runtime.Nodes.Expressions {
    public class RuntimeExpression : Nodes.RuntimeNode {
        public Expression Expression { get; protected set; }

        public RuntimeExpression(Expression expression, RuntimeScope scope) : base(expression.DefiningToken, scope) {
            Expression = expression;
        }

        public override RuntimeValueNode Evaluate() {
            return RuntimeNode.Create(Expression.Root, RuntimeScope).Evaluate();
        }
    }
}