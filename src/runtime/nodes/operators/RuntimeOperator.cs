using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Parser.Syntax.Expressions.Nodes.Operators.Comparison;
using BCake.Parser.Syntax.Expressions.Nodes.Operators.Logical;
using BCake.Runtime.Nodes.Value;
using BCake.Runtime.Nodes.Expressions;
using BCake.Runtime.Nodes.Operators.Comparison;
using BCake.Runtime.Nodes.Operators.Logical;

namespace BCake.Runtime.Nodes.Operators {
    public abstract class RuntimeOperator : RuntimeNode {
        public Operator Operator { get; protected set; }

        public RuntimeOperator(Operator op, RuntimeScope scope) : base(op.DefiningToken, scope) {
            Operator = op;
        }

        public new static RuntimeOperator Create(Node node, RuntimeScope scope) {
            switch (node) {
                case OperatorLogicalOr op: return new RuntimeOperatorLogicalOr(op, scope);

                case OperatorPlus op: return new RuntimeOperatorPlus(op, scope);
                case OperatorMinus op: return new RuntimeOperatorMinus(op, scope);
                case OperatorMultiply op: return new RuntimeOperatorMultiply(op, scope);
                case OperatorDivide op: return new RuntimeOperatorDivide(op, scope);
                case OperatorReturn op: return new RuntimeOperatorReturn(op, scope);

                case OperatorGreater op: return new RuntimeOperatorGreater(op, scope);
                case OperatorGreaterEqual op: return new RuntimeOperatorGreaterEqual(op, scope);
                case OperatorSmaller op: return new RuntimeOperatorSmaller(op, scope);
                case OperatorSmallerEqual op: return new RuntimeOperatorSmallerEqual(op, scope);
                case OperatorEqual op: return new RuntimeOperatorEqual(op, scope);
                case OperatorNotEqual op: return new RuntimeOperatorNotEqual(op, scope);

                case OperatorAssign op: return new RuntimeOperatorAssign(op, scope);
                case OperatorInvoke op: return new RuntimeOperatorInvoke(op, scope);
                case OperatorIndex op: return RuntimeOperatorInvoke.FromOperatorOverload(op, scope);
                case OperatorAccess op: return new RuntimeOperatorAccess(op, scope);
                case OperatorNew op: return new RuntimeOperatorNew(op, scope);

                case OperatorCast op: return new RuntimeOperatorCast(op, scope);
            }

            return null;
        }
    }
}