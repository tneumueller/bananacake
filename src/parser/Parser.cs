using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using BCake.Parser.Exceptions;
using BCake.Parser.Syntax.Expressions.Nodes;

namespace BCake.Parser
{
    public class Parser
    {
        private static string rxSeparators = @"(\s*([\(\).,:;{}])\s*|\s+([\(\).,:;{}])?\s*)";

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

            this.tokens = tokens.ToArray();
        }

        public Syntax.Namespace ParseRoot() {
            var globalNamespace = new Syntax.Namespace();
            string access = null, type = null, name = null;

            for (int i = 0; i < tokens.Length; ++i) {
                var token = tokens[i];

                switch (token.Value) {
                    case "public":
                    case "private":
                        if (access != null) throw new UnexpectedTokenException(token);
                        access = token.Value;
                        break;

                    case "namespace":
                        type = token.Value;
                        break;

                    case "{":
                        if (type == null || name == null) throw new UnexpectedTokenException(token);
                        if (access == null) access = "public";

                        var beginScope = i;
                        i = findClosingScope(tokens, i);

                        if (type == "namespace") {
                            globalNamespace.Scope.Declare(
                                new Syntax.Namespace(
                                    globalNamespace,
                                    access,
                                    name,
                                    tokens.Skip(beginScope + 1).Take(i - beginScope - 1).ToArray()
                                )
                            );

                            access = type = name = null;
                        }
                        break;

                    default: {
                            if (token.Value.Trim().Length < 1) break;
                            if (name != null) throw new UnexpectedTokenException(token);

                            Match m;
                            name = "";
                            
                            while (true) {
                                if (SymbolNode.CouldBeIdentifier(token.Value.Trim(), out m)) name += m.Value;
                                else throw new UnexpectedTokenException(token);

                                if (i + 2 < tokens.Length && tokens[i + 1].Value == ".") {
                                    name += ".";
                                    i += 2;
                                    token = tokens[i];
                                } else break;
                            }

                            break;
                        }
                }
            }

            return globalNamespace;
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
    }


    public class Token {
        public string Value, FilePath;
        public int Line, Column;
    }
}