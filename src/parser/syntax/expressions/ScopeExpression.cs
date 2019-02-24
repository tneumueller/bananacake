using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace BCake.Parser.Syntax.Expressions {
    public class ScopeExpression : Expression {
        public Nodes.Node Subscope { get; protected set; }

        private ScopeExpression(Token token, Scopes.Scope scope, Nodes.Node root, Nodes.Node subscope) : base(token, scope, root) {
            DefiningToken = token;
            Root = root;
            Subscope = subscope;

            Scope = new Scopes.Scope(scope);
        }

        public new static ScopeExpression Parse(Scopes.Scope scope, Token[] tokens, Nodes.Node subscope) {
            tokens = tokens.Where(t => t.Value.Trim().Length > 0).ToArray();

            if (tokens.Length < 1) return null;

            var t0 = tokens.FirstOrDefault();
            switch (t0.Value) {
                case "while": {
                    var conditionExpressionBegin = 1;
                    var conditionExpressionEnd = Parser.findClosingScope(tokens, conditionExpressionBegin);
                    if (conditionExpressionEnd < 0) throw new Exceptions.UnexpectedTokenException(tokens[1]);

                    return new ScopeExpression(
                        tokens[1],
                        scope,
                        new Nodes.WhileLoopNode(
                            tokens[1],
                            scope,
                            Expression.Parse(
                                scope,
                                tokens.Skip(conditionExpressionBegin + 1).Take(conditionExpressionEnd - conditionExpressionBegin - 1).ToArray()
                            )
                        ),
                        subscope
                    );
                }

                case "if": {
                    var conditionExpressionBegin = 1;
                    var conditionExpressionEnd = Parser.findClosingScope(tokens, conditionExpressionBegin);
                    if (conditionExpressionEnd < 0) throw new Exceptions.UnexpectedTokenException(tokens[1]);

                    return new ScopeExpression(
                        tokens[1],
                        scope,
                        new Nodes.IfConditionNode(
                            tokens[1],
                            scope,
                            Expression.Parse(
                                scope,
                                tokens.Skip(conditionExpressionBegin + 1).Take(conditionExpressionEnd - conditionExpressionBegin - 1).ToArray()
                            )
                        ),
                        subscope
                    );
                }

                default:
                    throw new Exceptions.UnexpectedTokenException(t0);
            }
        }
    }
}