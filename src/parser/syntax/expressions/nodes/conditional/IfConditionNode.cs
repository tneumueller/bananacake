using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace BCake.Parser.Syntax.Expressions.Nodes.Conditional {
    public class IfConditionNode : ConditionalNode {
        public IfConditionNode(Token token, Scopes.Scope scope, Expressions.Expression expression) : base(token, scope, expression) {}
    }
}