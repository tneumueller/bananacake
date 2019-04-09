using BCake.Parser.Syntax.Expressions;
using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;

namespace BCake.Runtime.Nodes {
    public class RuntimeScopeNode : RuntimeNode {
        public ScopeNode ScopeNode { get; protected set; }

        public RuntimeScopeNode(RuntimeScope parent, ScopeNode scopeNode)
            : base(scopeNode.DefiningToken, new RuntimeScope(parent, scopeNode.Scope)) {
            ScopeNode = scopeNode;
        }

        public new static RuntimeScopeNode Create(Node node, RuntimeScope scope) {
            switch (node) {
                case ScopeNode n: return new RuntimeScopeNode(scope, n);
            }

            return null;
        }

        public override RuntimeValueNode Evaluate() {
            foreach (var e in ScopeNode.Expressions) {
                RuntimeValueNode val;
                
                switch (e) {
                    case ScopeExpression s: {
                        val = new RuntimeScopeExpression(
                            e as ScopeExpression,
                            RuntimeScope
                        ).Evaluate();

                        if (val != null) return val;
                        break;
                    }

                    default: {
                        val = RuntimeNode.Create(e.Root, RuntimeScope).Evaluate();

                        if (e.Root is OperatorReturn) return val;
                        else if (e.Root is ScopeNode && val != null) return val; 
                        break;
                    }
                }
            }
            return null;
        }
    }
}