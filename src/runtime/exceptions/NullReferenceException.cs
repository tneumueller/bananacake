using BCake.Parser.Syntax.Types;

namespace BCake.Runtime.Exceptions {
    public class NullReferenceException : RuntimeException {
        public NullReferenceException(Type symbol, BCake.Parser.Token token)
            : base($"Tried accessing symbol {symbol.Name}, but it is null", token)
        {}
    }
}