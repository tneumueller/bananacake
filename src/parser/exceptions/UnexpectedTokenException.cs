namespace BCake.Parser.Exceptions {
    public class UnexpectedTokenException : System.Exception {
        public UnexpectedTokenException(BCake.Parser.Token token)
            : base($"Error: Unexpected token \"{token.Value}\"\n\tat {token.FilePath}({token.Line},{token.Column})")
        {}
    }
}