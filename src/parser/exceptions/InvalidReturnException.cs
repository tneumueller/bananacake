using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Exceptions {
    public class InvalidReturnException : TokenException {
        public InvalidReturnException(BCake.Parser.Token token)
            : base($"Error: Invalid return statement - there is nothing to return from", token)
        {}
    }
}