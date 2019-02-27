using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Types;
using BCake.Parser.Syntax.Expressions;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Operators;

namespace BCake.Runtime.Nodes {
    public abstract class RuntimeNode {
        public BCake.Parser.Token DefiningToken { get; protected set; }
        public RuntimeScope RuntimeScope { get; protected set; }

        public RuntimeNode(BCake.Parser.Token token, RuntimeScope scope) {
            DefiningToken = token;
            RuntimeScope = scope;
        }

        public static RuntimeNode Create(Node node, RuntimeScope scope) {
            RuntimeNode n;

            if ((n = RuntimeValueNode.Create(node, scope)) != null) return n;
            if ((n = RuntimeOperator.Create(node, scope)) != null) return n;
            if ((n = RuntimeSymbolNode.Create(node, scope)) != null) return n;

            return null; // todo handle this case property (exception?)
        }

        public abstract Nodes.Value.RuntimeValueNode Evaluate();
    }
}