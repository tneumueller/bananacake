using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;
using BCake.Parser.Syntax.Expressions.Nodes.Operators.Comparison;
using BCake.Parser.Exceptions;

namespace BCake.Parser.Syntax.Expressions {
    public class Expression {
        private static readonly Type[] OperatorPrecedence = new Type[] {
            typeof(OperatorReturn),

            // assigment
            typeof(OperatorAssign),
            
            // comparison
            typeof(OperatorGreater),

            // arithmetic
            typeof(OperatorPlus),
            typeof(OperatorMinus),
            typeof(OperatorMultiply),
            typeof(OperatorDivide),

            typeof(OperatorNew)
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
            tokens = tokens.Where(t => t.Value.Trim().Length > 0).ToArray();

            if (tokens.Length < 1) return null;

            for (int i = 0; i < OperatorPrecedence.Length; ++i) {
                var op = OperatorPrecedence[i];
                var opMeta = Operator.GetOperatorMetadata(op) ?? throw new Exception("Invalid operator definition - no metadata provided");

                var opPos = tokens.Select((t, index) => new { t.Value, index = index + 1 })
                    .TakeWhile(pair => pair.Value.Trim() != opMeta.Symbol)
                    .Select(pair => pair.index)
                    .LastOrDefault();

                if (opPos >= tokens.Length) continue;

                if (opMeta.Left == OperatorSymbolAttribute.OperatorParameterType.None) {
                    if (opPos > 0) throw new UnexpectedTokenException(tokens[opPos]);
                    return new Expression(
                        tokens[0],
                        Scope,
                        Operator.Parse(Scope, op, tokens[0], new Token[] {}, tokens.Skip(1).ToArray())
                    );
                } else {
                    return new Expression(
                        tokens[0],
                        Scope,
                        Operator.Parse(Scope, op, tokens[0], tokens.Take(opPos).ToArray(), tokens.Skip(opPos + 1).ToArray())
                    );
                }
            }

            if (tokens.Length > 2) {
                var t0 = tokens[0];
                var t1 = tokens[1];

                if (Nodes.SymbolNode.CouldBeIdentifier(t0.Value, out var m0)) {
                    var symbol = Scope.GetSymbol(t0.Value);
                    if (symbol == null) throw new UndefinedSymbolException(t0, t0.Value, Scope);

                    if ((symbol is Types.FunctionType || symbol is Types.ClassType) && t1.Value == "(") {
                        // function call or constructor call
                        var argListClose = Parser.findClosingScope(tokens, 1);

                        return new Expression(
                            t0,
                            Scope,
                            OperatorInvoke.Parse(
                                Scope,
                                Expression.Parse(Scope, tokens.Take(1).ToArray()),
                                tokens.Skip(2).Take(argListClose - 2).ToArray()
                            )
                        );
                    }
                }

            } else if (tokens.Length == 2) {
                var t0 = tokens[0];
                var t1 = tokens[1];

                if (Nodes.SymbolNode.CouldBeIdentifier(t0.Value, out var m0)
                    && Nodes.SymbolNode.CouldBeIdentifier(t1.Value, out var m1)
                ) {
                    // this could be an identifier, so it might be the type of a declaration
                    var symbol = Scope.GetSymbol(t0.Value);
                    if (symbol == null) throw new UndefinedSymbolException(t0, t0.Value, Scope);
                    if (symbol is Types.ClassType || symbol is Types.PrimitiveType) {
                        var newLocalVar = new Types.LocalVariableType(t1, Scope, symbol, t1.Value);
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