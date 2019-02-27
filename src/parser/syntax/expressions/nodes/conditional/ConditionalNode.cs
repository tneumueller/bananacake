using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace BCake.Parser.Syntax.Expressions.Nodes.Conditional {
    public abstract class ConditionalNode : Node {
        public Expressions.Expression Expression { get; protected set; }
        public Scopes.Scope Scope { get; protected set; }

        public ConditionalNode(Token token, Scopes.Scope scope, Expressions.Expression expression) : base(token) {
            Expression = expression;

            if (Expression.ReturnType != Nodes.Value.BoolValueNode.Type)
                throw new Exceptions.TypeException(Expression.DefiningToken, Expression.ReturnType, Nodes.Value.BoolValueNode.Type);

            Scope = new Scopes.Scope(scope);
        }
    }
}