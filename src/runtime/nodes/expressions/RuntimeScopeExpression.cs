using BCake.Parser.Syntax.Expressions;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Conditional;
using BCake.Parser.Syntax.Expressions.Nodes;

namespace BCake.Runtime.Nodes.Expressions {
    public class RuntimeScopeExpression : RuntimeExpression {
        public RuntimeScopeExpression(ScopeExpression expression, RuntimeScope scope) : base(expression, scope) {
            Expression = expression;
        }

        public override RuntimeValueNode Evaluate() {
            var scopeEx = Expression as ScopeExpression;
            return RuntimeConditionalNode.Create(
                scopeEx.Root, 
                RuntimeScope,
                RuntimeNode.Create(
                    scopeEx.Subscope,
                    RuntimeScope
                )
            ).Evaluate();
        }
    }
}