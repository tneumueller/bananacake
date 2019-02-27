using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Conditional;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;
using BCake.Runtime.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;

namespace BCake.Runtime.Nodes.Conditional {
    public abstract class RuntimeConditionalNode : RuntimeNode {
        public ConditionalNode Node { get; protected set; }

        public RuntimeConditionalNode(RuntimeScope parent, ConditionalNode node) : base(node.DefiningToken, new RuntimeScope(parent, node.Scope)) {
            Node = node;
        }

        public static RuntimeConditionalNode Create(Node node, RuntimeScope scope, RuntimeNode subscope) {
            switch (node) {
                case IfConditionNode n: return new RuntimeIfConditionNode(scope, n, subscope);
            }

            return null;
        }
    }
}