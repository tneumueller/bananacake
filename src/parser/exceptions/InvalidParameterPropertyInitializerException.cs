using System.Linq;
using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Exceptions {
    public class InvalidParameterPropertyInitializerException : TokenException {
        public InvalidParameterPropertyInitializerException(BCake.Parser.Token[] tokens)
                : base($"Invalid property initializer parameter", tokens.FirstOrDefault())
        {}

        public InvalidParameterPropertyInitializerException(FunctionType.InitializerParameterType param, string message)
                : base(
                    $"Invalid property initializer parameter \"{param.Name}\"" + (message != null
                        ? $"\n\t{message}"
                        : "")
                    ,
                    param.DefiningToken
                )
        {}
        public InvalidParameterPropertyInitializerException(FunctionType.InitializerParameterType param)
                : this(param, null)
        {}
    }
}