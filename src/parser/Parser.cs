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
using BCake.Parser.Syntax.Expressions.Nodes.Value;

namespace BCake.Parser
{
    public class Parser
    {
        private static string rxSeparators = @"(\s*([\(\).,:;{}""])\s*|\s+([\(\).,:;{}""])?\s*)";

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
                    if (separator == "\"") {
                        // beginning of string literal
                        var partUntrimmed = parts[i].Value;
                        var trimmedFront = partUntrimmed.Length - partUntrimmed.TrimStart().Length;
                        var str = findString(content, parts[i].Index + trimmedFront, out var lineBreaks, out var column, out var end);
                        if (str == null) throw new EndOfFileException(new Token {
                            FilePath = Filename,
                            Line = line,
                            Column = lineBreaks > 0 ? column : parts[i].Index - lineBegin + 1
                        });

                        tokens.Add(new Token {
                            Value = str,
                            FilePath = Filename,
                            Line = line,
                            Column = lineBreaks > 0 ? column : parts[i].Index - lineBegin + 1
                        });

                        line += lineBreaks;
                        if (lineBreaks > 0) lineBegin = end - column;

                        while (parts[i].Index < end) i++;
                        i--;
                    } else {
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
            }

            this.tokens = tokens.Where(t => t.Value.Trim().Length > 0).ToArray();
        }

        public Syntax.Namespace ParseRoot() {
            var globalNamespace = new Syntax.Namespace();
            Parser.ParseTypes(globalNamespace.Scope, tokens, new string[] { "namespace", "class", "function" });
            return globalNamespace;
        }

        public static void ParseTypes(Scope targetScope, Token[] tokens, string[] allowedTypes) {
            Access access = Access.@default;
            string type = null, name = null;
            Syntax.Types.Type valueType = null;
            FunctionType.ParameterType[] argList = null;

            for (int i = 0; i < tokens.Length; ++i) {
                var token = tokens[i];

                switch (token.Value) {
                    case "public":
                    case "protected":
                    case "private":
                        if (access != Access.@default) throw new UnexpectedTokenException(token);
                        access = accessFromString(token);
                        break;

                    case "void":
                        if (!allowedTypes.Contains("function")) throw new UnexpectedTokenException(token);
                        valueType = NullValueNode.Type;
                        break;

                    case "namespace":
                    case "class":
                    case "cast":
                        if (!allowedTypes.Contains(token.Value)) throw new UnexpectedTokenException(token);
                        type = token.Value;
                        break;

                    case "(":
                        if (type == null) type = "function";
                        if (type == "function" || type == "cast") {
                            if (!allowedTypes.Contains(type)) throw new UnexpectedTokenException(token);

                            if (type == "function") {
                                parseFunction(
                                    valueType,
                                    name, out name,
                                    out argList,
                                    targetScope,
                                    i, out i,
                                    tokens
                                );
                            } else if (type == "cast") {
                                parseCaster(
                                    valueType,
                                    name, out name,
                                    out argList,
                                    targetScope,
                                    i, out i,
                                    tokens
                                );
                            }
                        }
                        else throw new UnexpectedTokenException(token);
                        break;

                    case "{":
                        if (type == null || name == null) throw new UnexpectedTokenException(token);
                        if (access == Access.@default) access = Access.@public;

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
                        } else if (type == "function" || type == "cast") {
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
                            argList = null;
                        }

                        access = Access.@default;
                        type = name = null;
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

                            access = Access.@default;
                            name = type = null;
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

        private static Access accessFromString(Token t) {
            switch (t.Value) {
                case "public": return Access.@public;
                case "private": return Access.@private;
                default: throw new UnexpectedTokenException(t);
            }
        }

        private static void parseFunction(
            Syntax.Types.Type valueType,
            string _name, out string name,
            out FunctionType.ParameterType[] argList,
            Scope targetScope,
            int _tokenIndex, out int tokenIndex,
            Token[] tokens
        ){
            tokenIndex = _tokenIndex;
            name = _name;

            var token = tokens[tokenIndex];

            if (valueType == null) throw new UnexpectedTokenException(token);
                            
            if (name == null && valueType == targetScope.Type) {
                name = "!constructor"; // the ! is used as a kind of "escape" because it is impossible for a user created function to contain a ! in its name
            } else if (name == null) {
                throw new UnexpectedTokenException(token);
            }

            var argListBegin = tokenIndex;
            tokenIndex = Parser.findClosingScope(tokens, tokenIndex);

            // +1 in Take to include closing bracket, makes things easier in the parse method
            argList = FunctionType.ParseArgumentList(targetScope, tokens.Skip(argListBegin + 1).Take(tokenIndex - argListBegin).ToArray());

            if (name != "!constructor") {
                foreach (var param in argList) {
                    if (param is FunctionType.InitializerParameterType) {
                        throw new Exceptions.InvalidParameterPropertyInitializerException(
                            param as FunctionType.InitializerParameterType,
                            "initializer parameters are only allowed in constructors"
                        );
                    }
                }
            }
        }

        private static void parseCaster(
            Syntax.Types.Type valueType,
            string _name, out string name,
            out FunctionType.ParameterType[] argList,
            Scope targetScope,
            int _tokenIndex, out int tokenIndex,
            Token[] tokens
        ) {
            tokenIndex = _tokenIndex;
            name = $"!as_{ valueType.Name }";

            parseFunction(
                valueType,
                name, out name,
                out argList,
                targetScope,
                tokenIndex, out tokenIndex,
                tokens
            );

            if (argList.Length > 0) throw new Exceptions.InvalidCasterDefinitionException(
                argList.First().DefiningToken,
                "Casters may not have parameters"
            );
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

        public static string findString(string content, int startPos, out int lineBreaks, out int column, out int end) {
            // currently unused but will be used in the future for different kinds of strings
            // e.g. strings with a $ prefix where variables can be interpolated
            var mode = StringMode.String; 
            var escapeNext = false;
            var result = "";
            lineBreaks = 0;
            column = 0;
            end = -1;

            var prefix = "";
            var stringBegin = content.IndexOf("\"", startPos);
            if (stringBegin - startPos > 0) {
                prefix = content.Substring(startPos, stringBegin - startPos);
            }

            for (int i = stringBegin + 1; i < content.Length; ++i) {
                var c = content[i];

                switch (c) {
                    case '"':
                        if (!escapeNext) {
                            end = i + 1;
                            return prefix + "\"" + result + "\"";
                        }
                        escapeNext = false;
                        break;

                    case '\\':
                        if (!escapeNext) {
                            escapeNext = true;
                            continue;
                        }
                        break;

                    case '\n':
                        lineBreaks++;
                        column = 0;
                        break;
                }

                column++;
                result += c;
            }

            return null;
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

        public enum StringMode {
            None = 0,
            String
        }
    }

    public class Token {
        public string Value, FilePath;
        public int Line, Column;
    }
}