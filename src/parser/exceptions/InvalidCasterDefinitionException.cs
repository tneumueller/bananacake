using System.Linq;
using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Exceptions {
    public class InvalidCasterDefinitionException : TokenException {
        public InvalidCasterDefinitionException(Token token, string message)
                : base(
                    $"Invalid caster definition" + (message != null
                        ? $"\n\t{message}"
                        : "")
                    ,
                    token
                )
        {}
    }
}