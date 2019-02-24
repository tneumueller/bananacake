namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    public abstract class ComparisonOperator : Operator {
        public override Types.Type ReturnType {
            get => Value.BoolValueNode.Type;
        }
    }
}