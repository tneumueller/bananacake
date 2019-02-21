namespace BCake.Parser.Syntax.Expressions.Nodes {
    public abstract class Node {
        public virtual Types.Type ReturnType {
            get => null;
        }
    }
}