using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Exceptions {
    public class EndOfFileException : TokenException {
        public EndOfFileException(BCake.Parser.Token token)
            : base($"Error: Unexpected end of file", token)
        {}
    }
}