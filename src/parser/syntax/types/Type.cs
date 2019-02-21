namespace BCake.Parser.Syntax.Types {
    public class Type {
        public Namespace Namespace { get; protected set; }
        public string Access { get; protected set; }
        public string Name { get; protected set; }
        public virtual string FullName { get { return Namespace.Name + "." + Name; } }
        public Token DefiningToken { get; protected set; }
    }
}