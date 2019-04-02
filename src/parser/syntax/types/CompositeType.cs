using BCake.Parser.Syntax.Expressions.Nodes.Operators;

namespace BCake.Parser.Syntax.Types {
    public class CompositeType : Type {
        public OperatorAccess OperatorAccess { get; protected set; }

        public CompositeType(Scopes.Scope scope, OperatorAccess operatorAccess)
            : base(scope, operatorAccess.ReturnType.Name, null) {
            OperatorAccess = operatorAccess;
        }

        public static T Resolve<T>(Types.Type type) where T : Types.Type {
            return Resolve(type) as T;
        }
        public static Type Resolve(Types.Type type) {
            if (type is CompositeType) return (type as CompositeType).OperatorAccess.ReturnType;
            else return type;
        }
    }
}