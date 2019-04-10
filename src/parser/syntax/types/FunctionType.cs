using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BCake.Parser.Exceptions;
using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Operators;

namespace BCake.Parser.Syntax.Types {
    public class FunctionType : ComplexType {
        public Type Parent { get; protected set; }
        public Type ReturnType { get; protected set; }
        public override string FullName { get { return Parent.FullName + ":" + Name; } }
        public ScopeNode Root { get; protected set; }
        public ParameterType[] Parameters { get; protected set; }

        protected FunctionType(Type returnType, string name, ParameterType[] parameters)
                : base(Namespace.Global.Scope, name, "public") {
            ReturnType = returnType;
            Parameters = parameters;
        }

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
            bool propertyInitializer = false;
            List<Token> propertyInitializerTokens = null;

            string name = null;
            Type type = null;
            var arguments = new List<ParameterType>();

            for (int i = 0; i < tokens.Length; ++i) {
                var token = tokens[i];

                switch (token.Value) {
                    case ")":
                    case ",":
                        if (propertyInitializer) {
                            var exp = Expressions.Expression.Parse(scope, propertyInitializerTokens.ToArray());
                            
                            var opAccess = exp.Root as OperatorAccess;
                            if (opAccess == null) {
                                throw new InvalidParameterPropertyInitializerException(propertyInitializerTokens.ToArray());
                            }

                            var member = opAccess.ReturnSymbol as MemberVariableType;
                            if (member == null) {
                                throw new InvalidParameterPropertyInitializerException(propertyInitializerTokens.ToArray());
                            }

                            arguments.Add(new InitializerParameterType(propertyInitializerTokens.FirstOrDefault(), member));
                            propertyInitializer = false;
                        } else {
                            if (type == null && name == null) break;
                            if (type == null || name == null) throw new UnexpectedTokenException(token);
                            arguments.Add(new ParameterType(token, type, name));
                            name = null;
                            type = null;
                        }
                        break;

                    case "this":
                        propertyInitializer = true;
                        propertyInitializerTokens = new List<Token>() { token };
                        break;

                    default:
                        if (propertyInitializer) {
                            propertyInitializerTokens.Add(token);
                            break;
                        }

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

        public class InitializerParameterType : ParameterType {
            public MemberVariableType Member { get; protected set; }
            public InitializerParameterType(Token token, MemberVariableType member)
                    : base(token, member.Type, member.Name) {
                Member = member;
            }
        }
    }
}