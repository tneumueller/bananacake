namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    [OperatorSymbol(
        Symbol = "=",
        Left = OperatorSymbolAttribute.OperatorParameterType.LValue
    )]
    public class OperatorAssign : Operator, IRValue {
        public OperatorAssign() {
            System.Console.WriteLine("New OperatorAssign");
        }
    }
}