using BCake.Runtime.Nodes;
using BCake.Runtime.Nodes.Value;
using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;

namespace BCake.Runtime.Nodes {
    public class RuntimeSymbolNode : RuntimeNode {
        public SymbolNode SymbolNode { get; protected set; }

        public RuntimeSymbolNode(RuntimeScope parent, SymbolNode node) : base(node.DefiningToken, parent) {
            SymbolNode = node;
        }

        public new static RuntimeNode Create(Node node, RuntimeScope scope) {
            if (node is SymbolNode) {
                return new RuntimeSymbolNode(scope, node as SymbolNode);
            }

            return null;
        }

        public override RuntimeValueNode Evaluate() {
            return RuntimeScope.GetValue(SymbolNode.Symbol.Name);
        }
    }
}