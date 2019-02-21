using System;

namespace BCake.Parser.Syntax.Expressions.Nodes {
    public class SymbolNode : Node {
        public string SymbolName { get; protected set; }

        public SymbolNode(string symbolName) {
            SymbolName = symbolName;

            Console.WriteLine("New symbol node " + SymbolName);
        }

        public static SymbolNode Parse(Token token) {
            ValueNode node;

            Console.WriteLine("Parsing symbol node from " + token.Value);
            return new SymbolNode(token.Value);

            return null;
        }
    }
}