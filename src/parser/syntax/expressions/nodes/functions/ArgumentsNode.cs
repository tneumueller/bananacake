using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace BCake.Parser.Syntax.Expressions.Nodes.Functions {
    public class ArgumentsNode : Node {
        public Argument[] Arguments { get; protected set; }

        public ArgumentsNode() {
            System.Console.WriteLine("New ArgumentsNode");
        }

        public static ArgumentsNode Parse(Scopes.Scope scope, Token[] tokens) {
            var pos = 0;
            var arguments = new List<Argument>();

            while (true) {
                var paramTokens = tokens.Skip(pos).TakeWhile(t => t.Value != ",").ToArray();
                if (paramTokens.Length < 1) break;

                var paramExpr = Expression.Parse(scope, paramTokens);
                arguments.Add(new Argument(paramExpr));

                pos += paramTokens.Length + 1;
            }

            var arg = new ArgumentsNode();
            arg.Arguments = arguments.ToArray();
            return arg;
        }

        public class Argument {
            public Expression Expression { get; protected set; }

            public Argument(Expression expression) {
                Expression = expression;
            }
        }
    }
}