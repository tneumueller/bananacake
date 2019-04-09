using BCake.Parser.Syntax.Types;

namespace BCake.Runtime.Exceptions {
    public class OutOfBoundsException : RuntimeException {
        public OutOfBoundsException(int index, Type symbol, BCake.Parser.Token token)
            : base($"Index ${index} out of bounds for symbol {symbol.Name}", token)
        {}

        public OutOfBoundsException(int index, object value, BCake.Parser.Token token)
            : base($"Index ${index} out of bounds for value {value}", token)
        {}
    }
}