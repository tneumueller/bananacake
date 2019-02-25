namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    [OperatorSymbol(
        Symbol = "new",
        Left = OperatorSymbolAttribute.OperatorParameterType.None
    )]
    public class OperatorNew : Operator, IRValue {
        public Types.FunctionType ParentFunction { get; protected set; }

        public OperatorNew() {
            System.Console.WriteLine("New OperatorNew");
        }

        public override void OnCreated(Token token, Scopes.Scope scope) {
            
        }
    }
}