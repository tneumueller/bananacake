using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BCake.Parser.Exceptions;
using BCake.Parser.Syntax.Expressions.Nodes;

namespace BCake.Parser.Syntax.Types {
    public class FunctionType : ComplexType {
        public Type Parent { get; protected set; }
        public Type ReturnType { get; protected set; }
        public override string FullName { get { return Parent.FullName + ":" + Name; } }
        public Expressions.Expression[] Expressions { get; protected set; }
        public ParameterType[] Parameters { get; protected set; }

        public FunctionType(Token token, Type parent, string access, Type returnType, string name, ParameterType[] arguments, Token[] tokens) {
            DefiningToken = token;
            Parent = parent;
            Access = access;
            ReturnType = returnType;
            Name = name;
            Parameters = arguments;
            this.tokens = tokens;

            Scope = new Scopes.Scope(parent, this);

            var argListStr = string.Join(", ", arguments.Select(a => $"{a.Type.FullName} {a.Name}"));
            Console.WriteLine($"New function {Access} {ReturnType.FullName} {FullName}({argListStr})");
        }

        public static ParameterType[] ParseArgumentList(Scopes.Scope scope, Token[] tokens) {
            string name = null;
            Type type = null;
            var arguments = new List<ParameterType>();

            for (int i = 0; i < tokens.Length; ++i) {
                var token = tokens[i];

                switch (token.Value) {
                    case ")":
                    case ",":
                        if (type == null && name == null) break;
                        if (type == null || name == null) throw new UnexpectedTokenException(token);
                        arguments.Add(new ParameterType(type, name));
                        name = null;
                        type = null;
                        break;

                    default:
                        if (token.Value.Trim().Length < 1) break;
                        if (!SymbolNode.CouldBeIdentifier(token.Value.Trim(), out var m))
                            throw new UnexpectedTokenException(token);

                        if (type == null) {
                            type = scope.GetSymbol(token.Value)
                                ?? throw new Exceptions.UndefinedSymbolException(token, token.Value, scope);
                        } else if (name == null) {
                            name = token.Value;
                        } else {
                            throw new UnexpectedTokenException(token);
                        }
                        break;
                }
            }

            return arguments.ToArray();
        }

        public override void ParseInner() {
            var pos = 0;
            var expressions = new List<Expressions.Expression>();
            var tokens = this.tokens.Where(t => t.Value.Trim().Length > 0);
            
            while (true) {
                var expTokens = tokens.Skip(pos).TakeWhile(t => t.Value.Trim() != ";");
                var expLength = expTokens.Count();

                if (expLength == 0) break;

                expressions.Add(BCake.Parser.Syntax.Expressions.Expression.Parse(Scope, expTokens.ToArray()));
                pos += expLength + 1;
            }

            Expressions = expressions.ToArray();
        }

        public class ParameterType : Type {
            public Type Type { get; protected set; }
            public ParameterType(Type type, string name) {
                Type = type;
                Name = name;
            }
        }
    }
}