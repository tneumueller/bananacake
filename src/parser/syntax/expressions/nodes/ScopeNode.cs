using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace BCake.Parser.Syntax.Expressions.Nodes {
    public class ScopeNode : Node {
        public Token DefiningToken { get; protected set; }
        public Expressions.Expression[] Expressions { get; protected set; }
        public Scopes.Scope Scope { get; protected set; }

        private ScopeNode(Token token, Scopes.Scope scope, Expressions.Expression[] expressions) {
            DefiningToken = token;
            Expressions = expressions;

            Scope = scope;
        }

        public static ScopeNode Parse(Token token, Scopes.Scope parentScope, Token[] tokens) {
            var expressionDelimiters = new string[] {";", "{"};

            Console.WriteLine("Parsing scopenode");
            foreach (var t in tokens) Console.Write(t.Value);
            Console.WriteLine();

            var scope = new Scopes.Scope(parentScope);

            var pos = 0;
            var expressions = new List<Expressions.Expression>();
            tokens = tokens.Where(t => t.Value.Trim().Length > 0).ToArray();
            
            while (true) {
                if (pos >= tokens.Length) break;
                
                var expTokens = tokens.Skip(pos).TakeWhile(t => !expressionDelimiters.Contains(t.Value.Trim()));
                var expLength = expTokens.Count();

                var delimiter = tokens.Skip(pos + expLength).Take(1).FirstOrDefault();
                if (delimiter.Value == "{") {
                    // then a statement has not been terminated by a ; which is illegal
                    if (expLength > 0) throw new Exceptions.UnexpectedTokenException(delimiter);
                    
                    var subscopeBegin = pos + expLength;
                    var subscopeEnd = Parser.findClosingScope(tokens, subscopeBegin);
                    if (subscopeEnd == -1) throw new Exceptions.UnexpectedTokenException(delimiter);

                    var subscopeTokens = tokens.Skip(subscopeBegin + 1).Take(subscopeEnd - subscopeBegin - 1).ToArray();
                    expressions.Add(
                        new Expression(
                            delimiter,
                            scope,
                            ScopeNode.Parse(delimiter, parentScope, subscopeTokens)
                        )
                    );
                    pos = subscopeEnd + 1;
                } else {
                    expressions.Add(BCake.Parser.Syntax.Expressions.Expression.Parse(scope, expTokens.ToArray()));
                    pos += expLength + 1;
                }
            }

            return new ScopeNode(token, scope, expressions.ToArray());
        }
    }
}