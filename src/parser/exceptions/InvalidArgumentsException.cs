using System;
using System.Linq;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;

namespace BCake.Parser.Exceptions {
    public class InvalidArgumentsException : TokenException {
        public InvalidArgumentsException(
            BCake.Parser.Token token,
            BCake.Parser.Syntax.Types.FunctionType function,
            BCake.Parser.Syntax.Expressions.Nodes.Functions.ArgumentsNode.Argument[] provided
        )
            : base(
                $"Error: Invalid parameters provided for function {function.FullName}:\n\texpects {function.Parameters.Length}: ({FormatParamList(function.Parameters)})\n\tgiven {provided.Length}: ({FormatArgumentList(provided)})",
                token
            )
        {}

        private static string FormatParamList(BCake.Parser.Syntax.Types.FunctionType.ParameterType[] ps) {
            return string.Join(", ", ps.Select(p => p.Type.FullName));
        }

        private static string FormatArgumentList(BCake.Parser.Syntax.Expressions.Nodes.Functions.ArgumentsNode.Argument[] args) {
            return string.Join(", ", args.Select(a => a.Expression.ReturnType.FullName));
        }
    }
}