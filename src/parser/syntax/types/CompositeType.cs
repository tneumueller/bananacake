using BCake.Parser.Syntax.Expressions.Nodes.Operators;

namespace BCake.Parser.Syntax.Types {
    public class CompositeType : Type {
        public OperatorAccess OperatorAccess { get; protected set; }

        public CompositeType(Scopes.Scope scope, OperatorAccess operatorAccess)
            : base(scope, operatorAccess.ReturnType.Name) {
            OperatorAccess = operatorAccess;
        }
    }
}