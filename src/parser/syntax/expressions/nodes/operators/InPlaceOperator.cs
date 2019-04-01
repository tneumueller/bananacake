using System;
using BCake.Parser.Syntax.Scopes;

using OperatorHandler = System.Func<
    BCake.Parser.Syntax.Scopes.Scope,
    BCake.Parser.Token,
    BCake.Parser.Syntax.Scopes.Scope,
    bool
>;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    public class InPlaceOperator {
        public OperatorHandler Handler;

        public InPlaceOperator(OperatorHandler handler) {
            Handler = handler;
        }
    }
}