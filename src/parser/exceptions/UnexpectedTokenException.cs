namespace BCake.Parser.Exceptions {
    public class UnexpectedTokenException : TokenException {
        public UnexpectedTokenException(BCake.Parser.Token token)
            : base($"Error: Unexpected token \"{token.Value}\"", token)
        {}
    }
}