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
        private static readonly string[] bracketsOpen = new string[] { "(", "[", "{", "<" };
        private static readonly string[] bracketsClose = new string[] { ")", "]", "}", ">" };
        private static readonly Type[] OperatorPrecedence = new Type[] {

            // functions
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
            typeof(OperatorInvoke),
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

            for (int i = 0; i < OperatorPrecedence.Length; ++i) {
                var op = OperatorPrecedence[i];
                var opMeta = Operator.GetOperatorMetadata(op) ?? throw new Exception("Invalid operator definition - no metadata provided");
                var reverse = opMeta.Direction == OperatorAttribute.EvaluationDirection.RightToLeft;

                var tempTokens = tokens;
                if (reverse) tempTokens = tempTokens.Reverse().ToArray();

                var bracketIndent = 0;
                var opPos = tempTokens
                    .Select((t, index) => {
                        var bracketIndentBefore = bracketIndent;
                        if (bracketsOpen.ToList().Contains(t.Value)) bracketIndent += reverse ? -1 : 1;
                        if (bracketsClose.ToList().Contains(t.Value)) bracketIndent += reverse ? 1 : -1;
                        
                        return new {
                            t.Value,
                            index = index + 1,
                            bracketIndent = reverse ? bracketIndent : bracketIndentBefore
                        };
                    })
                    .TakeWhile(pair => pair.Value.Trim() != opMeta.Symbol || pair.bracketIndent != 0)
                    .Select(pair => pair.index)
                    .LastOrDefault();

                if (opPos >= tempTokens.Length) continue;
                if (reverse) opPos = tempTokens.Length - 1 - opPos;

                var tokensLeft = tokens.Take(opPos).ToArray();
                var tokensRight = tokens.Skip(opPos + 1).ToArray();

                // if (reverse) {
                //     var temp = tokensLeft;
                //     tokensLeft = tokensRight.Reverse().ToArray();
                //     tokensRight = temp.Reverse().ToArray();
                // }

                var handler = GetParsePreflight(op);
                if (handler != null) {
                    var info = new OperatorParseInfo {
                        OperatorPosition = opPos,
                        Tokens = tokens,
                        TokensLeft = tokensLeft,
                        TokensRight = tokensRight
                    };

                    // if the handler returns false, we continue with the next operator
                    if (!(bool)handler.Invoke(null, new object[] { info })) continue;
                }

                if (opMeta.Left == OperatorAttribute.ParameterType.None) {
                    if (opPos > 0) throw new UnexpectedTokenException(tempTokens[opPos]);
                    return new Expression(
                        tempTokens[0],
                        scope,
                        Operator.Parse(scope, typeSource, op, tempTokens[0], new Token[] {}, tempTokens.Skip(1).ToArray())
                    );
                } else {
                    return new Expression(
                        tempTokens[0],
                        scope,
                        Operator.Parse(scope, typeSource, op, tempTokens[0], tokensLeft, tokensRight)
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
                    var symbol = Types.CompositeType.Resolve(SymbolNode.GetSymbol(typeSource, t0));
                    if (symbol == null) throw new UndefinedSymbolException(t0, t0.Value, scope);

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

        private static System.Reflection.MethodInfo GetParsePreflight(Type op) {
            var methods = op.GetMethods();
            foreach (var m in methods) {
                if (!m.IsStatic) continue;

                var attr = m.GetCustomAttributes(
                    typeof(OperatorParsePreflight),
                    true
                ).FirstOrDefault() as OperatorParsePreflight;

                if (attr != null) return m;
            }

            return null;
        }
    }
}