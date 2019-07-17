namespace BCake.Parser.Syntax.Types {
    public abstract class Type {
        public Scopes.Scope Scope { get; protected set; }
        public Access Access { get; protected set; }
        public string Name { get; protected set; }
        public object DefaultValue { get; protected set; }
        public virtual string FullName {
            get { 
                if (Scope == null) return Name == "null" ? "void" : Name;

                var typeName = Scope.FullName;
                if (typeName == null || typeName.Length < 1) return Name;
                else return typeName + "." + Name;
            }
        }
        public Token DefiningToken { get; protected set; }

        public Type(Scopes.Scope scope, string name, object defaultValue) {
            Scope = new Scopes.Scope(scope, this);
            // Scope.Declare(this, "this");

            Name = name;
            DefaultValue = defaultValue;
        }
        public Type(Scopes.Scope scope, Access access, string name) : this(scope, access, name, null) {}
        public Type(Scopes.Scope scope, Access access, string name, object defaultValue)
            : this(scope, name, defaultValue) {
            Access = access;
        }
    }
}