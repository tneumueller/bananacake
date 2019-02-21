using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    public abstract class Operator : Node {
        public Expression Left { get; protected set; }
        public Expression Right { get; protected set; }

        public static string GetOperatorSymbol(Type t) {
            var opSymbolAttr = t.GetCustomAttributes(
                typeof(OperatorSymbolAttribute),
                true
            ).FirstOrDefault() as OperatorSymbolAttribute;

            if (opSymbolAttr == null) return null;
            return opSymbolAttr.Symbol;
        }

        public static Node Parse(Type opType, Token[] left, Token[] right) {
            var op = (Operator)Activator.CreateInstance(opType);
            op.Left = Expression.Parse(left);
            op.Right = Expression.Parse(right);

            return op;
        }
    }
}