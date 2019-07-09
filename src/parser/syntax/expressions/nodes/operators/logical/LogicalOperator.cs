namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    public abstract class LogicalOperator : Operator {
        public override Types.Type ReturnType {
            get => Value.BoolValueNode.Type;
        }
    }
}