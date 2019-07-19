using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Exceptions {
    public class InvalidCastException : TokenException {
        public InvalidCastException(Type left, Type right, Token token)
            : base($"Cannot cast {left.FullName} to {right.FullName} because there is no matching cast method", token)
        {}
    }
}