using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Runtime.Nodes.Value;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;

namespace BCake.Runtime.Nodes {
    public class RuntimeScopeNode : RuntimeNode {
        public ScopeNode ScopeNode { get; protected set; }

        public RuntimeScopeNode(RuntimeScope parent, ScopeNode scopeNode) : base(scopeNode.DefiningToken, new RuntimeScope(parent, scopeNode.Scope)) {
            ScopeNode = scopeNode;
        }

        public override RuntimeValueNode Evaluate() {
            foreach (var e in ScopeNode.Expressions) {
                var val = RuntimeNode.Create(e.Root, RuntimeScope).Evaluate();
                if (e.Root is OperatorReturn) return val;
                else if (e.Root is ScopeNode && val != null) return val; 
            }
            return null;
        }
    }
}