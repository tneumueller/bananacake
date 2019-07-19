using System.Linq;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    [Operator(
        Symbol = "[",
        OverloadableName = "index",
        CheckReturnTypes = false,
        Direction = OperatorAttribute.EvaluationDirection.RightToLeft
    )]
    public class OperatorIndex : OverloadableOperator, IRValue {
        protected override Expression ParseRight(Scopes.Scope scope, Token[] tokens, Scopes.Scope typeSource) {
            // remove the closing square bracket from the end
            return base.ParseRight(scope, tokens.Take(tokens.Length - 1).ToArray(), typeSource);
        }

        [OperatorParsePreflight]
        public static bool ParsePreflight(OperatorParseInfo info) {
            var tokens = info.Tokens;

            if (tokens.Length <= 2) return false;

            var left = string.Join("", info.TokensLeft.Select(t => t.Value));
            if (!SymbolNode.CouldBeIdentifier(left, out var m)) {
                return false;
            }

            return true;
        }
    }
}