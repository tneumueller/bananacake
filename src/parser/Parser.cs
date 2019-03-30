using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using BCake.Parser.Exceptions;
using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Scopes;
using BCake.Parser.Syntax.Types;

namespace BCake.Parser
{
    public class Parser
    {
        private static string rxSeparators = @"(\s*([\(\),:;{}])\s*|\s+([\(\).,:;{}])?\s*)";

        public string Filename { get; private set; }
        private Token[] tokens;

        public Parser(string filename) {
            Filename = filename;

            var content = File.ReadAllText(Filename);
            SplitTokens(content);
        }

        private void SplitTokens(string content) {
            var parts = Regex.Matches(content, rxSeparators).Cast<Match>().ToArray();
            var tokens = new List<Token>();
            int line = 1, lineBegin = 0;

            for (int i = 0; i <= parts.Length; ++i) {
                int posBegin, length;

                if (i == 0) {
                    posBegin = 0;
                    length = parts[i].Index;
                }
                else if (i == parts.Length) {
                    posBegin = parts[i - 1].Index + parts[i - 1].Length;
                    length = content.Length - posBegin;
                }
                else {
                    posBegin = parts[i - 1].Index + parts[i - 1].Length;
                    length = parts[i].Index - posBegin;
                }

                tokens.Add(new Token {
                    Value = content.Substring(posBegin, length),
                    FilePath = Filename,
                    Line = line,
                    Column = posBegin - lineBegin + 1
                });
                if (i < parts.Length) {
                    var separator = parts[i].Value.Trim();
                    if (separator.Length > 0) tokens.Add(new Token {
                        Value = separator,
                        FilePath = Filename,
                        Line = line,
                        Column = parts[i].Index - lineBegin + 1
                    });

                    var linebreaks = parts[i].Value.Count(c => c == '\n');
                    if (linebreaks > 0) {
                        line += linebreaks;
                        lineBegin = parts[i].Index + parts[i].Value.LastIndexOf('\n') + 1;
                    }
                }
            }

            this.tokens = tokens.Where(t => t.Value.Trim().Length > 0).ToArray();
        }

        public Syntax.Namespace ParseRoot() {
            var globalNamespace = new Syntax.Namespace();
            Parser.ParseTypes(globalNamespace.Scope, tokens, new string[] { "namespace", "class", "function" });
            return globalNamespace;
        }

        public static void ParseTypes(Scope targetScope, Token[] tokens, string[] allowedTypes) {
            string access = null, type = null, name = null;
            Syntax.Types.Type valueType = null;
            FunctionType.ParameterType[] argList = null;

            for (int i = 0; i < tokens.Length; ++i) {
                var token = tokens[i];

                switch (token.Value) {
                    case "public":
                    case "protected":
                    case "private":
                        if (access != null) throw new UnexpectedTokenException(token);
                        access = token.Value;
                        break;

                    case "void":
                        type = "function";
                        if (!allowedTypes.Contains(type)) throw new UnexpectedTokenException(token);
                        valueType = targetScope.GetSymbol(token.Value) ?? throw new UndefinedSymbolException(token, token.Value, targetScope);
                        break;

                    case "namespace":
                    case "class":
                        if (!allowedTypes.Contains(token.Value)) throw new UnexpectedTokenException(token);
                        type = token.Value;
                        break;

                    case "(":
                        if (type == null) {
                            type = "function";
                            if (!allowedTypes.Contains(type)) throw new UnexpectedTokenException(token);

                            if (valueType == null) throw new UnexpectedTokenException(token);
                            if (name == null && valueType == targetScope.Type) {
                                name = "!constructor"; // the ! is used as a kind of "escape" because it is impossible for a user created function to contain a ! in its name
                            } else if (name == null) {
                                throw new UnexpectedTokenException(token);
                            }

                            var argListBegin = i;
                            i = Parser.findClosingScope(tokens, i);

                            // +1 in Take to include closing bracket, makes things easier in the parse method
                            argList = FunctionType.ParseArgumentList(targetScope, tokens.Skip(argListBegin + 1).Take(i - argListBegin).ToArray());
                        }
                        else throw new UnexpectedTokenException(token);
                        break;

                    case "{":
                        if (type == null || name == null) throw new UnexpectedTokenException(token);
                        if (access == null) access = "public";

                        var beginScope = i;
                        i = findClosingScope(tokens, i);

                        if (type == "namespace") {
                            targetScope.Declare(
                                new Syntax.Namespace(
                                    targetScope,
                                    access,
                                    name,
                                    tokens.Skip(beginScope + 1).Take(i - beginScope - 1).ToArray()
                                )
                            );
                        } else if (type == "class") {
                            targetScope.Declare(
                                new Syntax.Types.ClassType(
                                    targetScope,
                                    access,
                                    name,
                                    tokens.Skip(beginScope + 1).Take(i - beginScope - 1).ToArray()
                                )
                            );
                        } else if (type == "function") {
                            var newFunction = new FunctionType(
                                tokens[i - 1],
                                targetScope.Type,
                                access,
                                valueType,
                                name,
                                argList,
                                tokens.Skip(beginScope + 1).Take(i - beginScope - 1).ToArray()
                            );
                            targetScope.Declare(newFunction);
                        }

                        access = type = name = null;
                        valueType = null;
                        break;

                    case ";":
                        if (type == null) {
                            type = "variable";
                            if (!allowedTypes.Contains(type)) throw new UnexpectedTokenException(token);
                            if (name == null || valueType == null) throw new UnexpectedTokenException(token);

                            var newMember = new MemberVariableType(
                                tokens[i - 1],
                                targetScope.Type,
                                access,
                                valueType,
                                name
                            );
                            targetScope.Declare(newMember);

                            access = name = type = null;
                            valueType = null;
                        }
                        else throw new UnexpectedTokenException(token);
                        break;

                    default: {
                            var temp = "";
                            
                            while (true) {
                                if (SymbolNode.CouldBeIdentifier(token.Value.Trim(), out var m)) temp += m.Value;
                                else throw new UnexpectedTokenException(token);

                                if (i + 2 < tokens.Length && tokens[i + 1].Value == ".") {
                                    temp += ".";
                                    i += 2;
                                    token = tokens[i];
                                } else break;
                            }

                            if (type != "class" && type != "namespace" && valueType == null) {
                                valueType = targetScope.GetSymbol(temp) ?? throw new UndefinedSymbolException(token, temp, targetScope);
                            } else {
                                if (name != null) throw new UnexpectedTokenException(token);
                                name = temp;
                            }

                            break;
                        }
                }
            }
        }

        public static int findClosingScope(Token[] tokens, int startTokenIndex) {
            var token = tokens[startTokenIndex];
            string closing = null;
            var level = 0;

            switch (token.Value.Trim()) {
                case "{":
                    closing = "}";
                    break;
                case "(":
                    closing = ")";
                    break;
            }

            for (int i = startTokenIndex; i < tokens.Length; ++i) {
                if (tokens[i].Value == token.Value) level++;
                if (tokens[i].Value == closing) level--;
                if (level == 0) return i;
            }

            return -1;
        }

        public static int findListItemEnd(Token[] tokens, int startTokenIndex) {
            var brackets = new string[] {"(", "{", "[", "<"};

            for (int i = startTokenIndex; i < tokens.Length; ++i) {
                var token = tokens[i];
                if (brackets.Contains(token.Value)) i = findClosingScope(tokens, i);
                else if (token.Value == ",") return i;
            }

            return -1;
        }
    }


    public class Token {
        public string Value, FilePath;
        public int Line, Column;
    }
}