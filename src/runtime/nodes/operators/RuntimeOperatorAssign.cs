using BCake.Parser.Syntax.Types;
using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators {
    public class RuntimeOperatorAssign : RuntimeOperator {
        public RuntimeOperatorAssign(OperatorAssign op, RuntimeScope scope) : base(op, scope) {}

        public override RuntimeValueNode Evaluate() {
            var symbolNode = Operator.Left.Root as SymbolNode;
            Type symbol;
            RuntimeScope assignmentScope = RuntimeScope;

            if (symbolNode == null && Operator.Left.Root is OperatorAccess) {
                var opAccess = Operator.Left.Root as OperatorAccess;
                symbol = opAccess.ReturnSymbol;

                var symbolValue = new RuntimeOperatorAccess(
                    opAccess,
                    RuntimeScope
                ).Evaluate();
                assignmentScope = symbolValue.RuntimeScope;
            } else {
                symbol = symbolNode.Symbol;
            }

            var r = new RuntimeExpression(
                Operator.Right,
                RuntimeScope
            ).Evaluate();

            assignmentScope.SetValue(symbol.Name, r);
            return r;
        }
    }
}