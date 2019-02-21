namespace BCake.Parser.Exceptions {
    public class DuplicateDeclarationException : System.Exception {
        public DuplicateDeclarationException(
            BCake.Parser.Token token,
            BCake.Parser.Syntax.Types.Type member
        )
            : base($"Error: Duplicate declaration - the symbol \"{member.FullName}\" is already defined at {member.DefiningToken.FilePath}({member.DefiningToken.Line},{member.DefiningToken.Column})\n\tat {token.FilePath}({token.Line},{token.Column})")
        {}
    }
}