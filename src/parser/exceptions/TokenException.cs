using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Exceptions {
    public class TokenException : System.Exception {
        public TokenException(string message, BCake.Parser.Token token)
            : base($"{message}\n\tat {token.FilePath}({token.Line},{token.Column})")
        {}
    }
}