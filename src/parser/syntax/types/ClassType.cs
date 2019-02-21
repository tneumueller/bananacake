using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Exceptions;
using System.Text.RegularExpressions;

namespace BCake.Parser.Syntax.Types {
    public class ClassType : ComplexType {
        public ClassType(Namespace ns, string access, string name, BCake.Parser.Token[] tokens) {
            Access = access;
            Name = name;
            Namespace = ns;
            this.tokens = tokens;

            Scope = new Scopes.Scope();
        }

        public override void ParseInner(Namespace[] allNamespaces, Type[] allTypes) {
            string access = null, name = null, symbolType = null, valueType = null;
            FunctionType.ArgumentType[] argList = null;
            // variables AS WELL AS functions and other constructs
            var memberFunctions = new List<FunctionType>();

            for (int i = 0; i < tokens.Length; ++i) {
                var token = tokens[i];

                switch (token.Value) {
                    case "private":
                    case "protected":
                    case "public":
                        access = token.Value;
                        break;

                    case "void":
                        symbolType = "function";
                        valueType = token.Value;
                        break;

                    case "(":
                        if (symbolType == null) {
                            symbolType = "function";

                            if (name == null || valueType == null) throw new UnexpectedTokenException(token);

                            var argListBegin = i;
                            i = Parser.findClosingScope(tokens, i);

                            // +1 in Take to include closing bracket, makes things easier in the parse method
                            argList = FunctionType.ParseArgumentList(tokens.Skip(argListBegin + 1).Take(i - argListBegin).ToArray());
                        }
                        else throw new UnexpectedTokenException(token);
                        break;

                    case "{":
                        if (symbolType == null
                            || symbolType != "function"
                            || name == null
                            || valueType == null) {
                            throw new UnexpectedTokenException(token);
                        }
                        
                        var beginBody = i;
                        i = Parser.findClosingScope(tokens, i);
                        
                        var newFunction = new FunctionType(
                            tokens[i - 1],
                            Namespace,
                            this,
                            access,
                            valueType,
                            name,
                            argList,
                            tokens.Skip(beginBody + 1).Take(i - beginBody - 1).ToArray()
                        );
                        Scope.RegisterMember(newFunction);
                        memberFunctions.Add(newFunction);

                        access = name = symbolType = valueType = null;
                        break;

                    case ";":
                        if (symbolType == null) {
                            symbolType = "variable";

                            if (name == null || valueType == null) throw new UnexpectedTokenException(token);

                            var newMember = new MemberVariableType(
                                tokens[i - 1],
                                Namespace,
                                this,
                                access,
                                valueType,
                                name
                            );
                            Scope.RegisterMember(newMember);

                            access = name = symbolType = valueType = null;
                        }
                        else throw new UnexpectedTokenException(token);
                        break;

                    default:
                        if (token.Value.Trim().Length < 1) break;
                        if (!new Regex(Parser.rxIdentifier).Match(token.Value).Success)
                            throw new UnexpectedTokenException(token);

                        if (valueType == null) {
                            valueType = token.Value;
                        } else {
                            if (name != null) throw new UnexpectedTokenException(token);
                            name = token.Value;
                        }
                        break;
                }
            }

            foreach (var m in memberFunctions) {
                m.ParseInner(allNamespaces, allTypes);
            }
        }
    }
}