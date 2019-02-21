using BCake.Parser.Syntax.Expressions.Nodes.Operators;

namespace BCake.Parser.Exceptions {
    public class InvalidArgumentException : System.Exception {
        public InvalidArgumentException(
            BCake.Parser.Token token,
            OperatorSymbolAttribute.OperatorParameterType expectedType
        )
            : base($"Error: The expression on the left hand side of an assignment operation in invalid - {expectedType} expected\n\tat {token.FilePath}({token.Line},{token.Column})")
        {}
    }
}