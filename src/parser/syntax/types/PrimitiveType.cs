using BCake.Parser.Syntax.Expressions.Nodes.Value;

namespace BCake.Parser.Syntax.Types {
    public class PrimitiveType : Type {
        public PrimitiveType(Scopes.Scope parent, string name, object defaultValue)
            : base(parent, name, defaultValue) {
        }
    }
}