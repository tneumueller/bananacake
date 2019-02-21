namespace BCake.Parser.Exceptions {
    public class UndefinedSymbolException : TokenException {
        public UndefinedSymbolException(
            BCake.Parser.Token token,
            string name,
            BCake.Parser.Syntax.Scopes.Scope scope
        )
            : base($"Error: Undefined symbol - the symbol \"{name}\" has not been declared in this scope\n\tin scope {scope.FullName}", token)
        {}
    }
}