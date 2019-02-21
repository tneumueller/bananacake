using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Exceptions {
    public class TypeException : TokenException {
        public TypeException(
            BCake.Parser.Token token,
            Type t1, Type t2
        )
            : base($"Error: The type {t1?.FullName ?? "void"} cannot be implicitly converted to type {t2?.FullName ?? "void"}", token)
        {}
    }
}