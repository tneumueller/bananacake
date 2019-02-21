using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Exceptions;

namespace BCake.Parser.Syntax {
    public class Namespace {
        public string Access { get; private set; }
        public string Name { get; private set; }
        private BCake.Parser.Token[] tokens;

        public Namespace(string access, string name, BCake.Parser.Token[] tokens) {
            Access = access;
            Name = name;
            this.tokens = tokens;
        }

        public Types.ComplexType[] ParseSymbols(Namespace[] allNamespaces) {
            string access = null, name = null, type = null;
            var complexTypes = new List<Types.ComplexType>();

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
                            complexTypes.Add(
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

            return complexTypes.ToArray();
        }
    }
}