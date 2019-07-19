using BCake.Parser.Syntax.Types;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;

namespace BCake.Parser.Exceptions {
    public class OperatorNotDefinedException<T> : TokenException {
        public OperatorNotDefinedException(Token token, Type type)
            : base($"Invalid use of operator \"{ Operator.GetOperatorMetadata<T>().Symbol }\" - no overload defined on type { type.FullName }", token)
        {}
    }
}