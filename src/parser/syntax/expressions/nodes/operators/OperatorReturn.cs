namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    [OperatorSymbol(
        Left = OperatorSymbolAttribute.OperatorParameterType.None
    )]
    public class OperatorReturn : Operator, IRValue {
        public Types.FunctionType ParentFunction { get; protected set; }

        public static OperatorReturn Create(Token token, Scopes.Scope scope, Token[] right) {
            var function = scope.GetClosestFunction() ?? throw new Exceptions.InvalidReturnException(token);
            var op = new OperatorReturn();
            op.Right = Expression.Parse(scope, right);
            op.ParentFunction = function;
            op.CheckRightReturnType(function.ReturnType);
            return op;
        }
    }
}