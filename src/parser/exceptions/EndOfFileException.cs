using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Exceptions {
    public class EndOfFileException : TokenException {
        public EndOfFileException(BCake.Parser.Token token)
            : base($"Unexpected end of file", token)
        {}
    }
}