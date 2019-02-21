using System;

namespace BCake.Parser.Syntax.Expressions.Nodes {
    public abstract class ValueNode : Node {
        public virtual object Value { get; protected set; }

        public static ValueNode Parse(Token token) {
            ValueNode node;

            Console.WriteLine("Parsing value node from " + token.Value);

            if ((node = Nodes.Value.IntValueNode.Parse(token)) != null) {
                return node;
            }

            return null;
        }
    }
}