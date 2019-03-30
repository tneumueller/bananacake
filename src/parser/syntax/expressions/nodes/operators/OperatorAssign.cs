namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    [Operator(
        Symbol = "=",
        Left = OperatorAttribute.ParameterType.LValue
    )]
    public class OperatorAssign : Operator, IRValue {
        public OperatorAssign() {
            System.Console.WriteLine("New OperatorAssign");
        }
    }
}