namespace BCake.Parser.Exceptions {
    public class DuplicateDeclarationException : TokenException {
        public DuplicateDeclarationException(
            BCake.Parser.Token token,
            BCake.Parser.Syntax.Types.Type member
        ) : base($"Duplicate declaration - the symbol \"{member.FullName}\" is already defined at {member.DefiningToken.FilePath}({member.DefiningToken.Line},{member.DefiningToken.Column})", token)
        {}
    }
}