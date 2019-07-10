using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Exceptions {
    public class TypeException : TokenException {
        public TypeException(
            BCake.Parser.Token token,
            Type t1, Type t2
        ) : base($"The type {t1?.FullName ?? "void"} cannot be implicitly converted to type {t2?.FullName ?? "void"}", token)
        {}

        public TypeException(
            BCake.Parser.Token token,
            Type t1, Type t2, Type castTo
        ) :base($"The type {t1?.FullName ?? "void"} cannot be implicitly converted to type {t2?.FullName ?? "void"}, but an explicit cast exists - you may want to use '{ token.Value } as { castTo.Name }'", token)
        {}
    }
}