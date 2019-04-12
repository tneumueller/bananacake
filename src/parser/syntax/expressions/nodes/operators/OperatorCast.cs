using BCake.Parser.Exceptions;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    [Operator(
        Symbol = "as",
        Direction = OperatorAttribute.EvaluationDirection.RightToLeft,
        CheckReturnTypes = false
    )]
    public class OperatorCast : Operator, IRValue {
        public override Types.Type ReturnType {
            get => (Right?.Root as SymbolNode).Symbol;
        }

        public override void OnCreated(Token token, Scopes.Scope scope) {
            if (!(Left?.Root is SymbolNode)) throw new UnexpectedTokenException(DefiningToken);
            if (!(Right?.Root is SymbolNode)) throw new UnexpectedTokenException(DefiningToken);

            var leftSymbol = (Left?.Root as SymbolNode).Symbol;
            var rightSymbol = (Right?.Root as SymbolNode).Symbol;

            Types.Type leftType;
            if (leftSymbol is Types.MemberVariableType) {
                leftType = (leftSymbol as Types.MemberVariableType).Type;
            } else if (leftSymbol is Types.LocalVariableType) {
                leftType = (leftSymbol as Types.LocalVariableType).Type;
            } else {
                throw new InvalidCastException(leftSymbol, rightSymbol, DefiningToken);
            }

            var casterFunction = leftType.Scope.GetSymbol($"!as_{ rightSymbol.Name }", true);
            if (casterFunction == null) {
                throw new InvalidCastException(leftSymbol, rightSymbol, DefiningToken);
            }
        }
    }
}