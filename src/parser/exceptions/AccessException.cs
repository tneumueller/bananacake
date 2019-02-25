namespace BCake.Parser.Exceptions {
    public class AccessException : TokenException {
        public AccessException(
            BCake.Parser.Token token,
            BCake.Parser.Syntax.Types.Type member,
            BCake.Parser.Syntax.Scopes.Scope sourceScope
        )
            : base($"Error: Cannot access {member.Access} symbol \"{member.FullName}\" from scope \"{sourceScope.FullName}\"", token)
        {}
    }
}