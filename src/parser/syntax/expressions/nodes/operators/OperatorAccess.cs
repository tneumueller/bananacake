using System.Linq;
using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    [Operator(
        Symbol = ".",
        Direction = OperatorAttribute.EvaluationDirection.RightToLeft,
        CheckReturnTypes = false
    )]
    public class OperatorAccess : Operator, IRValue {
        public Types.Type SymbolToAccess { get; protected set; }

        public override Types.Type ReturnType {
            get {
                switch (Right.Root) {
                    case SymbolNode n: return n.Symbol;
                    default: return Right.ReturnType;
                }
            }
        }

        public OperatorAccess() {
            System.Console.WriteLine("New OperatorAccess");
        }

        protected override Expression ParseRight(Scopes.Scope scope, Token[] tokens, Scopes.Scope typeSource) {
            if (Left == null) throw new System.Exception("Left hand side of access operator must not be null when parsing right hand side");

            var symbol = Expression.Parse(scope, tokens, Left.ReturnType.Scope);
            // if (symbol.ReturnType != )

            return symbol;
        }

        public override void OnCreated(Token token, Scopes.Scope scope) {
            var leftSymbol = Left.Root as SymbolNode;
            if (leftSymbol == null) return;

            // we need to treat the left hand side specifically, if it is not a type
            switch (leftSymbol.Symbol) {
                case Namespace t: return;
                case ClassType t: return;

                default:
                    SymbolToAccess = leftSymbol.Symbol;
                    break;
            }
        }
    }
}