using System.Linq;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    public abstract class OverloadableOperator : Operator {
        public Types.Type Target { get; protected set; }
        protected Expression _targetNode;
        protected Types.FunctionType _operatorFunction;
        public override Types.Type ReturnType {
            get => _operatorFunction.ReturnType;
        }

        /// <summary>
        /// checks if the left hand side type of the operator defines an overload (for the
        /// derived operator class with the name given in the Operator attribute)
        /// </summary>
        protected override Expression ParseLeft(Scopes.Scope scope, Token[] tokens, Scopes.Scope typeSource) {
            Types.Type symbol;

            _targetNode = Expression.Parse(scope, tokens, typeSource);

            switch (_targetNode.Root) {
                case SymbolNode s: symbol = s.Symbol; break;
                case OperatorAccess o: symbol = o.ReturnSymbol; break;
                default: symbol = _targetNode.ReturnType; break;
            }

            var opMeta = Operator.GetOperatorMetadata(this.GetType());
            var opName = $"!operator_{ opMeta.OverloadableName }";
            _operatorFunction = _targetNode.ReturnType.Scope.GetSymbol(opName, true) as Types.FunctionType;
            if (_operatorFunction == null) throw new Exceptions.OperatorNotDefinedException<OperatorIndex>(DefiningToken, _targetNode.ReturnType);

            return _targetNode;
        }

        /// <summary>
        /// checks if the overload found in ParseLeft takes the right hand side type
        /// as a parameter. If yes, remembers the fitting overload in _operatorFunction.
        /// (if there is no overload, this function will never be called)
        /// </summary>
        protected override Expression ParseRight(Scopes.Scope scope, Token[] tokens, Scopes.Scope typeSource) {
            var right = Expression.Parse(scope, tokens);

            Types.FunctionType overload = null;
            // no need to check _operatorFunction for null because that happens in ParseLeft
            var possibleOverloads = _operatorFunction.Overloads.Prepend(_operatorFunction);
            foreach (var o in possibleOverloads) {
                if (!o.ParameterListDiffers(new Types.Type[] { right.ReturnType })) {
                    overload = o;
                    break;
                }
            }

            if (overload == null) {
                throw new Exceptions.InvalidArgumentsException(
                    DefiningToken,
                    _operatorFunction,
                    new Functions.ArgumentsNode.Argument[] { new Functions.ArgumentsNode.Argument(right) }
                );
            }

            _operatorFunction = overload;
            return right;
        }

        public OperatorInvoke ToOperatorInvoke(Scopes.Scope scope) {
            return OperatorInvoke.FromOverloadableOperator(scope, this, _operatorFunction, _targetNode);
        }
    }
}