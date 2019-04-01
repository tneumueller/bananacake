using System;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    public struct OperatorParseInfo {
        public Token[] Tokens, TokensLeft, TokensRight;
        public int OperatorPosition;
    }
}