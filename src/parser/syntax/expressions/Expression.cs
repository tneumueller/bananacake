using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Syntax.Expressions.Nodes;
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
            typeof(OperatorGreaterEqual),
            typeof(OperatorSmaller),
            typeof(OperatorSmallerEqual),
            typeof(OperatorEqual),

            // arithmetic
            typeof(OperatorPlus),
            typeof(OperatorMinus),
            typeof(OperatorMultiply),
            typeof(OperatorDivide),

            typeof(OperatorNew),
            typeof(OperatorAccess)
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

        public static Expression Parse(Scopes.Scope scope, Token[] tokens, Scopes.Scope typeSource = null) {
            if (typeSource == null) typeSource = scope;

            if (tokens.Length < 1) return null;

            if (tokens.Length > 2) {
                var t0 = tokens[0];
                var t1 = tokens[1];

                if (SymbolNode.CouldBeIdentifier(t0.Value, out var m0) && t1.Value == "(") {
                    Types.Type symbol;
                    var _symbol = SymbolNode.GetSymbol(typeSource, t0);
                    if (_symbol == null) throw new UndefinedSymbolException(t0, t0.Value, scope);

                    if (_symbol is Types.CompositeType) symbol = (_symbol as Types.CompositeType).OperatorAccess.ReturnType;
                    else symbol = _symbol;

                    if ((symbol is Types.FunctionType || symbol is Types.ClassType)) {
                        // function call or constructor call
                        var argListClose = Parser.findClosingScope(tokens, 1);

                        return new Expression(
                            t0,
                            scope,
                            OperatorInvoke.Parse(
                                scope,
                                Expression.Parse(scope, tokens.Take(1).ToArray()),
                                tokens.Skip(2).Take(argListClose - 2).ToArray()
                            )
                        );
                    }
                }
            }

            for (int i = 0; i < OperatorPrecedence.Length; ++i) {
                var op = OperatorPrecedence[i];
                var opMeta = Operator.GetOperatorMetadata(op) ?? throw new Exception("Invalid operator definition - no metadata provided");
                var reverse = opMeta.Direction == OperatorAttribute.EvaluationDirection.RightToLeft;

                var tempTokens = tokens;
                if (reverse) tempTokens = tempTokens.Reverse().ToArray();

                var opPos = tempTokens
                    .Select((t, index) => new { t.Value, index = index + 1 })
                    .TakeWhile(pair => pair.Value.Trim() != opMeta.Symbol)
                    .Select(pair => pair.index)
                    .LastOrDefault();

                if (opPos >= tempTokens.Length) continue;

                var tokensLeft = tempTokens.Take(opPos).ToArray();
                var tokensRight = tempTokens.Skip(opPos + 1).ToArray();

                if (reverse) {
                    var temp = tokensLeft;
                    tokensLeft = tokensRight.Reverse().ToArray();
                    tokensRight = temp.Reverse().ToArray();
                }

                if (opMeta.Left == OperatorAttribute.ParameterType.None) {
                    if (opPos > 0) throw new UnexpectedTokenException(tempTokens[opPos]);
                    return new Expression(
                        tempTokens[0],
                        scope,
                        Operator.Parse(scope, op, tempTokens[0], new Token[] {}, tempTokens.Skip(1).ToArray())
                    );
                } else {
                    return new Expression(
                        tempTokens[0],
                        scope,
                        Operator.Parse(scope, op, tempTokens[0], tokensLeft, tokensRight)
                    );
                }
            }

            if (tokens.Length == 2) {
                var t0 = tokens[0];
                var t1 = tokens[1];

                if (SymbolNode.CouldBeIdentifier(t0.Value, out var m0)
                    && SymbolNode.CouldBeIdentifier(t1.Value, out var m1)
                ) {
                    // this could be an identifier, so it might be the type of a declaration
                    Types.Type symbol;
                    var _symbol = SymbolNode.GetSymbol(typeSource, t0);

                    if (_symbol == null) throw new UndefinedSymbolException(t0, t0.Value, scope);

                    if (_symbol is Types.CompositeType) symbol = (_symbol as Types.CompositeType).OperatorAccess.ReturnType;
                    else symbol = _symbol;

                    if (symbol is Types.ClassType || symbol is Types.PrimitiveType) {
                        var newLocalVar = new Types.LocalVariableType(t1, scope, symbol, t1.Value);
                        scope.Declare(newLocalVar);
                        return new Expression(t0, scope, SymbolNode.Parse(scope, t1));
                    }
                }
            } else if (tokens.Length == 1) {
                Nodes.Node node;
                var t = tokens[0];

                if ((node = Nodes.Value.ValueNode.Parse(t)) != null) return new Expression(t, scope, node);
                if ((node = SymbolNode.Parse(typeSource, t)) != null) return new Expression(t, scope, node);
                else throw new Exceptions.UndefinedSymbolException(t, t.Value, scope);
            }

            throw new Exceptions.UnexpectedTokenException(tokens[0]);
        }
    }
}