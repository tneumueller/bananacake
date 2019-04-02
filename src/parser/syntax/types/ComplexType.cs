namespace BCake.Parser.Syntax.Types {
    public abstract class ComplexType : Type {
        protected BCake.Parser.Token[] tokens;

        public ComplexType(Scopes.Scope scope, string name)
            : base(scope, name, null) {}
        public ComplexType(Scopes.Scope scope, string name, string access)
            : base(scope, name, access, null) {}

        public abstract void ParseInner();
    }
}