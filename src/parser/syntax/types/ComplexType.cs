namespace BCake.Parser.Syntax.Types {
    public abstract class ComplexType : Type {
        protected BCake.Parser.Token[] tokens;
        public BCake.Parser.Syntax.Scopes.Scope Scope;
        
        public abstract void ParseInner(Namespace[] allNamespaces, Type[] allTypes);
    }
}