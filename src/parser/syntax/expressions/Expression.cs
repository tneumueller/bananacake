using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Parser.Exceptions;

namespace BCake.Parser.Syntax.Expressions {
    public class Expression {
        private static readonly Type[] OperatorPrecedence = new Type[] {
            typeof(OperatorAssign),
            typeof(OperatorPlus)
        };

        public Token DefiningToken { get; protected set; }
        public Nodes.Node Root { get; protected set; }
        public Scopes.Scope Scope { get; protected set; }
        public Types.Type ReturnType {
            get => Root.ReturnType;
        }

        public Expression(Token token, Scopes.Scope scope, Nodes.Node root) {
            DefiningToken = token;
            Root = root;
            Scope = scope;
        }

        public static Expression Parse(Scopes.Scope Scope, Token[] tokens) {
            for (int i = 0; i < OperatorPrecedence.Length; ++i) {
                var op = OperatorPrecedence[i];
                var opSymbol = Operator.GetOperatorSymbol(op);

                var opPos = tokens.Select((t, index) => new { t.Value, index = index + 1 })
                    .TakeWhile(pair => pair.Value.Trim() != opSymbol)
                    .Select(pair => pair.index)
                    .LastOrDefault();

                if (opPos < tokens.Length) {
                    return new Expression(
                        tokens[0],
                        Scope,
                        Operator.Parse(Scope, op, tokens.Take(opPos).ToArray(), tokens.Skip(opPos + 1).ToArray())
                    );
                }
            }

            if (tokens.Length == 2) {
                var t0 = tokens[0];
                var t1 = tokens[1];

                if (Nodes.SymbolNode.CouldBeIdentifier(t0.Value, out var m0)
                    && Nodes.SymbolNode.CouldBeIdentifier(t1.Value, out var m1)
                ) {
                    // this could be an identifier, so it might be the type of a declaration
                    var symbol = Scope.GetSymbol(t0.Value);
                    if (symbol == null) throw new UndefinedSymbolException(t0, t0.Value, Scope);
                    if (symbol is Types.ClassType || symbol is Types.PrimitiveType) {
                        var newLocalVar = new Types.LocalVariableType(t1, Scope.Type, symbol, t1.Value);
                        Scope.Declare(newLocalVar);
                        return new Expression(t0, Scope, Nodes.SymbolNode.Parse(Scope, t1));
                    }
                }
            } else if (tokens.Length == 1) {
                Nodes.Node node;
                var t = tokens[0];

                if ((node = Nodes.ValueNode.Parse(t)) != null) return new Expression(t, Scope, node);
                if ((node = Nodes.SymbolNode.Parse(Scope, t)) != null) return new Expression(t, Scope, node);
                else throw new Exceptions.UndefinedSymbolException(t, t.Value, Scope);
            }

            throw new Exceptions.UnexpectedTokenException(tokens[0]);
        }
    }
}