using BCake.Parser.Syntax.Expressions.Nodes.Operators;

namespace BCake.Parser.Exceptions {
    public class InvalidArgumentException : TokenException {
        public InvalidArgumentException(
            BCake.Parser.Token token,
            OperatorSymbolAttribute.OperatorParameterType expectedType
        )
            : base($"Error: The expression on the left hand side of an assignment operation in invalid - {expectedType} expected", token)
        {}
    }
}