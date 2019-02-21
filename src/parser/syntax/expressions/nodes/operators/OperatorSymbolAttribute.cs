using System;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    public class OperatorSymbolAttribute : Attribute {
        public string Symbol;
        public OperatorParameterType Left, Right;

        public enum OperatorParameterType {
            RValue = 0,
            LValue,
            None
        }
    }
}