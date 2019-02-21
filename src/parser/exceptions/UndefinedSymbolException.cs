namespace BCake.Parser.Exceptions {
    public class UndefinedSymbolException : System.Exception {
        public UndefinedSymbolException(
            BCake.Parser.Token token,
            string name,
            BCake.Parser.Syntax.Scopes.Scope scope
        )
            : base($"Error: Undefined symbol - the symbol \"{name}\" has not been declared in this scope\n\tin scope {scope.FullName}\n\tat {token.FilePath}({token.Line},{token.Column})")
        {}
    }
}