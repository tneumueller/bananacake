namespace BCake.Parser.Exceptions {
    public class AccessViolationException : TokenException {
        public AccessViolationException(
            BCake.Parser.Token token,
            BCake.Parser.Syntax.Types.Type member,
            BCake.Parser.Syntax.Scopes.Scope sourceScope
        )
            : base($"Cannot access {member.Access} symbol \"{member.FullName}\" from current scope due to it's protection level", token)
        {}
    }
}