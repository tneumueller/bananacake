using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Conditional;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;
using BCake.Runtime.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;

namespace BCake.Runtime.Nodes.Conditional {
    public class RuntimeIfConditionNode : RuntimeConditionalNode {
        public RuntimeNode Subscope { get; protected set; }

        public RuntimeIfConditionNode(RuntimeScope parent, IfConditionNode ifNode, RuntimeNode subscope) : base(parent, ifNode) {
            Subscope = subscope;
        }

        public override RuntimeValueNode Evaluate() {
            var ifNode = Node as IfConditionNode;

            var condition_ = new RuntimeExpression(
                ifNode.Expression,
                RuntimeScope
            ).Evaluate();
            var condition = condition_  as RuntimeBoolValueNode;

            if ((bool)condition.Value) {
                return Subscope.Evaluate();
            }
            return null;
        }
    }
}