namespace BCake.Parser.Syntax.Types {
    public class PrimitiveType : Type {
        public PrimitiveType(Scopes.Scope scope, string name, object defaultValue)
            : base(scope, name, defaultValue) {}
    }
}