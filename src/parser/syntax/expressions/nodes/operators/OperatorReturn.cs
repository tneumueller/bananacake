namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    [Operator(
        Symbol = "return",
        Left = OperatorAttribute.ParameterType.None
    )]
    public class OperatorReturn : Operator, IRValue {
        public Types.FunctionType ParentFunction { get; protected set; }

        public OperatorReturn() {}

        public override void OnCreated(Token token, Scopes.Scope scope) {
            ParentFunction = scope.GetClosestFunction() ?? throw new Exceptions.InvalidReturnException(token);
            CheckRightReturnType(ParentFunction.ReturnType);
        }
    }
}