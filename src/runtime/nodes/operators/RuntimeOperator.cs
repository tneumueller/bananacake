using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;

namespace BCake.Runtime.Nodes.Operators {
    public abstract class RuntimeOperator : RuntimeNode {
        public Operator Operator { get; protected set; }

        public RuntimeOperator(Operator op, RuntimeScope scope) : base(op.DefiningToken, scope) {
            Operator = op;
        }

        public new static RuntimeOperator Create(Node node, RuntimeScope scope) {
            switch (node) {
                case OperatorPlus op: return new RuntimeOperatorPlus(op, scope);
                case OperatorMinus op: return new RuntimeOperatorMinus(op, scope);
                case OperatorMultiply op: return new RuntimeOperatorMultiply(op, scope);
                case OperatorDivide op: return new RuntimeOperatorDivide(op, scope);
                case OperatorReturn op: return new RuntimeOperatorReturn(op, scope);
                case OperatorAssign op: return new RuntimeOperatorAssign(op, scope);
                case OperatorInvoke op: return new RuntimeOperatorInvoke(op, scope);
            }

            return null;
        }
    }
}