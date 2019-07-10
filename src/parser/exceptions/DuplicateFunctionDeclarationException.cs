using System.Linq;

namespace BCake.Parser.Exceptions {
    public class DuplicateFunctionDeclarationException : TokenException {
        public DuplicateFunctionDeclarationException(
            BCake.Parser.Token token,
            BCake.Parser.Syntax.Types.FunctionType function
        ) : base($"Duplicate function declaration - an override of \"{function.FullName}\" with the same parameters ({FormatParamList(function.Parameters)}) is already defined at {function.DefiningToken.FilePath}({function.DefiningToken.Line},{function.DefiningToken.Column})", token)
        {}

        private static string FormatParamList(BCake.Parser.Syntax.Types.FunctionType.ParameterType[] ps) {
            return string.Join(", ", ps.Select(p => p.Type.FullName));
        }
    }
}