using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BCake.Parser.Syntax.Expressions.Nodes;

namespace BCake.Parser.Syntax.Expressions.Nodes {
    public class SymbolNode : Node, ILValue, IRValue {
        public static readonly string rxIdentifier = @"^[A-Za-z_][A-Za-z_0-9]*(.[A-Za-z_][A-Za-z_0-9]*)*$";
        public Types.Type Symbol { get; protected set; }
        public override Types.Type ReturnType {
            get {
                switch (Symbol) {
                    case Types.LocalVariableType t: return t.Type;
                    case Types.MemberVariableType t: return t.Type;
                    case Types.FunctionType t: return t.ReturnType;
                    case Types.FunctionType.ParameterType t: return t.Type;
                    case Types.ClassType t: return t;
                    case Namespace t: return t;
                }
                return null; // todo what now? does not make much sense
            }
        }

        public SymbolNode(Token token, Types.Type symbol) : base(token) {
            Symbol = symbol;

            Console.WriteLine("New symbol node " + Symbol.FullName);
        }

        public static bool CouldBeIdentifier(string s, out Match m) {
            m = Regex.Match(s, rxIdentifier);
            return m.Success;
        }

        public static SymbolNode Parse(Scopes.Scope scope, Token token) {
            var symbol = GetSymbol(scope, token);
            if (symbol == null) return null;
            return new SymbolNode(token, symbol);
        }

        public static Types.Type GetSymbol(Scopes.Scope scope, Token token) {
            var simpleSymbol = scope.GetSymbol(token.Value);
            if (simpleSymbol != null) return simpleSymbol;

            var compositeTypeTokens = new List<Token>();
            var parts = token.Value.Split(".");

            if (parts.Length > 1) throw new Exception($"FATAL Parser thought this type is composite but it is not: {token.Value}");

            foreach (var p in parts) {
                compositeTypeTokens.Add(new Token {
                    Column = token.Column,
                    Line = token.Line,
                    FilePath = token.FilePath,
                    Value = p
                });
                compositeTypeTokens.Add(new Token {
                    Column = token.Column,
                    Line = token.Line,
                    FilePath = token.FilePath,
                    Value = "."
                });
            }
            compositeTypeTokens.RemoveAt(compositeTypeTokens.Count - 1);
                
            var symbolExpression = Expression.Parse(scope, compositeTypeTokens.ToArray());

            return new Types.CompositeType(
                scope,
                symbolExpression.Root as Operators.OperatorAccess
            );
        }
    }
}