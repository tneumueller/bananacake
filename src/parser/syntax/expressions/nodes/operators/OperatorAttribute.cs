using System;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    public class OperatorAttribute : Attribute {
        public string Symbol;
        public ParameterType Left, Right;
        public EvaluationDirection Direction;
        public bool CheckReturnTypes = true;

        public enum ParameterType {
            RValue = 0,
            LValue,
            None
        }

        public enum EvaluationDirection {
            LeftToRight = 0,
            RightToLeft
        }
    }
}