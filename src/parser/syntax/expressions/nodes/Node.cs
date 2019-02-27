namespace BCake.Parser.Syntax.Expressions.Nodes {
    public abstract class Node {
        public Token DefiningToken { get; protected set; }

        public Node(Token token) {
            DefiningToken = token;
        }

        public virtual Types.Type ReturnType {
            get => null;
        }
    }
}