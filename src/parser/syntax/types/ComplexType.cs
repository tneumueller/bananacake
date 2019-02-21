namespace BCake.Parser.Syntax.Types {
    public abstract class ComplexType : Type {
        protected BCake.Parser.Token[] tokens;
        
        public abstract void ParseInner();
    }
}