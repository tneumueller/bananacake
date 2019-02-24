using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BCake.Parser.Exceptions;
using BCake.Parser.Syntax.Expressions.Nodes;

namespace BCake.Parser.Syntax.Types {
    public class ClassType : ComplexType {
        public override string FullName { get => Scope.FullName; }
        public ClassType(Scopes.Scope parent, string access, string name, BCake.Parser.Token[] tokens) {
            Access = access;
            Name = name;
            this.tokens = tokens;

            Scope = new Scopes.Scope(parent, this);
            Scope.Declare(this, "this");
        }

        public override void ParseInner() {
            string access = null, name = null, symbolType = null;
            Types.Type valueType = null;
            FunctionType.ParameterType[] argList = null;

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
                        valueType = Scope.GetSymbol(token.Value) ?? throw new UndefinedSymbolException(token, token.Value, Scope);
                        break;

                    case "(":
                        if (symbolType == null) {
                            symbolType = "function";

                            if (name == null || valueType == null) throw new UnexpectedTokenException(token);

                            var argListBegin = i;
                            i = Parser.findClosingScope(tokens, i);

                            // +1 in Take to include closing bracket, makes things easier in the parse method
                            argList = FunctionType.ParseArgumentList(Scope, tokens.Skip(argListBegin + 1).Take(i - argListBegin).ToArray());
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
                            this,
                            access,
                            valueType,
                            name,
                            argList,
                            tokens.Skip(beginBody + 1).Take(i - beginBody - 1).ToArray()
                        );
                        Scope.Declare(newFunction);
                        memberFunctions.Add(newFunction);

                        access = name = symbolType = null;
                        valueType = null;
                        break;

                    case ";":
                        if (symbolType == null) {
                            symbolType = "variable";

                            if (name == null || valueType == null) throw new UnexpectedTokenException(token);

                            var newMember = new MemberVariableType(
                                tokens[i - 1],
                                this,
                                access,
                                valueType,
                                name
                            );
                            Scope.Declare(newMember);

                            access = name = symbolType = null;
                            valueType = null;
                        }
                        else throw new UnexpectedTokenException(token);
                        break;

                    default:
                        if (token.Value.Trim().Length < 1) break;
                        if (!SymbolNode.CouldBeIdentifier(token.Value.Trim(), out var m))
                            throw new UnexpectedTokenException(token);

                        if (valueType == null) {
                            valueType = Scope.GetSymbol(token.Value) ?? throw new UndefinedSymbolException(token, token.Value, Scope);
                        } else {
                            if (name != null) throw new UnexpectedTokenException(token);
                            name = token.Value;
                        }
                        break;
                }
            }

            foreach (var m in memberFunctions) {
                m.ParseInner();
            }
        }
    }
}