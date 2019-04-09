namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    [Operator(
        Symbol = "new",
        Left = OperatorAttribute.ParameterType.None
    )]
    public class OperatorNew : Operator, IRValue {
        public Types.FunctionType ParentFunction { get; protected set; }

        public OperatorNew() {}

        public override void OnCreated(Token token, Scopes.Scope scope) {
            
        }
    }
}