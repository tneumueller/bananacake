namespace BCake.Parser.Syntax.Types {
    public abstract class Type {
        public Scopes.Scope Scope { get; protected set; }
        public string Access { get; protected set; }
        public string Name { get; protected set; }
        public virtual string FullName {
            get { 
                var typeName = Scope.FullName;
                if (typeName == null || typeName.Length < 1) return Name;
                else return typeName + "." + Name;
            }
        }
        public Token DefiningToken { get; protected set; }
    }
}