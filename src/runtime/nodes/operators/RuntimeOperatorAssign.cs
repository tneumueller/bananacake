using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators {
    public class RuntimeOperatorAssign : RuntimeOperator {
        public RuntimeOperatorAssign(OperatorAssign op, RuntimeScope scope) : base(op, scope) {}

        public override RuntimeValueNode Evaluate() {
            var symbolNode = Operator.Left.Root as SymbolNode;
            var r = new RuntimeExpression(
                Operator.Right,
                RuntimeScope
            ).Evaluate();

            RuntimeScope.SetValue(symbolNode.Symbol.Name, r);
            return r;
        }
    }
}