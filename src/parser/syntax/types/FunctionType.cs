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
        public ScopeNode Root { get; protected set; }
        public ParameterType[] Parameters { get; protected set; }

        public FunctionType(Token token, Type parent, string access, Type returnType, string name, ParameterType[] parameters, Token[] tokens)
            : base(null, name, access) {
            DefiningToken = token;
            Parent = parent;
            ReturnType = returnType;
            Parameters = parameters;
            this.tokens = tokens;

            Scope = new Scopes.Scope(parent.Scope, this);
            foreach (var p in parameters) p.SetScope(Scope);
            Scope.Declare(parameters);

            var argListStr = string.Join(", ", parameters.Select(a => $"{a.Type.FullName} {a.Name}"));
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
                        arguments.Add(new ParameterType(token, type, name));
                        name = null;
                        type = null;
                        break;

                    default:
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
            Root = ScopeNode.Parse(DefiningToken, Scope, tokens);
        }

        public class ParameterType : Type {
            public Type Type { get; protected set; }
            public ParameterType(Token token, Type type, string name)
                : base(null, name, type.DefaultValue) {
                DefiningToken = token;
                Type = type;
            }

            public void SetScope(Scopes.Scope s) {
                Scope = s;
            }
        }
    }
}