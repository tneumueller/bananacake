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

        public FunctionType(Token token, Type parent, string access, Type returnType, string name, ArgumentType[] arguments, Token[] tokens) {
            DefiningToken = token;
            Parent = parent;
            Access = access;
            ReturnType = returnType;
            Name = name;
            this.tokens = tokens;

            Scope = new Scopes.Scope(parent, this);

            var argListStr = string.Join(", ", arguments.Select(a => $"{a.Type} {a.Name}"));
            Console.WriteLine($"New function {Access} {ReturnType.FullName} {FullName}({argListStr})");
        }

        public static ArgumentType[] ParseArgumentList(Token[] tokens) {
            string type = null, name = null;
            var arguments = new List<ArgumentType>();

            for (int i = 0; i < tokens.Length; ++i) {
                var token = tokens[i];

                switch (token.Value) {
                    case ")":
                    case ",":
                        if (type == null && name == null) break;
                        if (type == null || name == null) throw new UnexpectedTokenException(token);
                        arguments.Add(new ArgumentType(type, name));
                        type = name = null;
                        break;

                    default:
                        if (token.Value.Trim().Length < 1) break;
                        if (!SymbolNode.CouldBeIdentifier(token.Value.Trim(), out var m))
                            throw new UnexpectedTokenException(token);

                        if (type == null) {
                            type = token.Value;
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
            
            while (true) {
                var expTokens = tokens.Skip(pos).TakeWhile(t => t.Value.Trim() != ";").Where(t => t.Value.Trim().Length > 0);
                var expLength = expTokens.Count();

                if (expLength == 0) break;

                expressions.Add(BCake.Parser.Syntax.Expressions.Expression.Parse(Scope, expTokens.ToArray()));
                pos += expLength + 1;
            }

            Expressions = expressions.ToArray();
        }

        public class ArgumentType : Type {
            public string Type { get; protected set; }
            public ArgumentType(string type, string name) {
                Type = type;
                Name = name;
            }
        }
    }
}