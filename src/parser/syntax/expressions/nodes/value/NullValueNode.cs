using System;
using System.Text.RegularExpressions;
using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Syntax.Expressions.Nodes.Value {
    public class NullValueNode : ValueNode {
        public static Types.PrimitiveType Type = new PrimitiveType(null, "null", null);

        public override Types.Type ReturnType {
            get => Type;
        }

        public NullValueNode(Token token) : base(token) {
            Value = null;
        }

        public new static ValueNode Parse(Token token) {
            if (token.Value == "null") return new NullValueNode(token);
            else return null;
        }
    }
}