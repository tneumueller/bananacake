using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;

namespace BCake.Parser.Syntax.Expressions {
    public class Expression {
        private static readonly Type[] OperatorPrecedence = new Type[] {
            typeof(OperatorAssign),
            typeof(OperatorPlus)
        };

        public Nodes.Node Root { get; protected set; }

        public Expression(Nodes.Node root) {
            Root = root;
        }

        public static Expression Parse(Token[] tokens) {
            for (int i = 0; i < OperatorPrecedence.Length; ++i) {
                var op = OperatorPrecedence[i];
                var opSymbol = Operator.GetOperatorSymbol(op);

                var opPos = tokens.Select((t, index) => new { t.Value, index = index + 1 })
                    .TakeWhile(pair => pair.Value.Trim() != opSymbol)
                    .Select(pair => pair.index)
                    .LastOrDefault();

                if (opPos < tokens.Length) {
                    return new Expression(
                        Operator.Parse(op, tokens.Take(opPos).ToArray(), tokens.Skip(opPos + 1).ToArray())
                    );
                }
            }

            if (tokens.Length == 1) {
                Nodes.Node node;

                if ((node = Nodes.ValueNode.Parse(tokens[0])) != null) return new Expression(node);
                if ((node = Nodes.SymbolNode.Parse(tokens[0])) != null) return new Expression(node);
            }

            throw new Exceptions.UnexpectedTokenException(tokens[0]);
        }
    }
}