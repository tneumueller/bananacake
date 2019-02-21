using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Exceptions;
using BCake.Parser.Syntax.Types;
using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Value;

namespace BCake.Parser.Syntax {
    public class Namespace : Types.ComplexType {
        public static Namespace Global { get; protected set; }

        public Namespace() {
            Access = "public";
            Scope = new Scopes.Scope();
            Global = this;

            InitPrimitives();
        }
        public Namespace(Types.Type parent, string access, string name, BCake.Parser.Token[] tokens) {
            Access = access;
            Name = name;
            this.tokens = tokens;

            Scope = new Scopes.Scope(parent);
        }

        public override void ParseInner() {
            string access = null, name = null, type = null;

            for (int i = 0; i < tokens.Length; ++i) {
                var token = tokens[i];

                switch (token.Value) {
                    case "class":
                        type = token.Value;
                        break;

                    case "public":
                    case "protected":
                    case "private":
                        if (access != null) throw new UnexpectedTokenException(token);
                        access = token.Value;
                        break;

                    case "{":
                        if (type == null || name == null) throw new UnexpectedTokenException(token);
                        // by default the class will get the same access level as the namespace
                        if (access == null) access = this.Access;

                        var beginScope = i;
                        i = Parser.findClosingScope(tokens, i);

                        if (type == "class") {
                            Scope.Declare(
                                new Types.ClassType(
                                    this,
                                    access,
                                    name,
                                    tokens.Skip(beginScope + 1).Take(i - beginScope - 1).ToArray()
                                )
                            );

                            access = type = name = null;
                        }
                        break;

                    default:
                        if (token.Value.Trim().Length < 1) break;

                        if (name != null) throw new UnexpectedTokenException(token);
                        name = token.Value;
                        break;
                }
            }
        }

        private void InitPrimitives() {
            Scope.Declare(
                IntValueNode.Type,
                BoolValueNode.Type
            );
        }
    }
}