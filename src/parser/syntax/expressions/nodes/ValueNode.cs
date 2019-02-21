using System;

namespace BCake.Parser.Syntax.Expressions.Nodes {
    public abstract class ValueNode : Node, IRValue {
        public virtual object Value { get; protected set; }

        public static ValueNode Parse(Token token) {
            ValueNode node;

            if ((node = Nodes.Value.IntValueNode.Parse(token)) != null) return node;
            if ((node = Nodes.Value.BoolValueNode.Parse(token)) != null) return node;

            return null;
        }
    }
}