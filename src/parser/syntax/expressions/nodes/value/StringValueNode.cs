using System;
using System.Text.RegularExpressions;
using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Syntax.Expressions.Nodes.Value {
    public class StringValueNode : ValueNode {
        public static Types.PrimitiveType Type = new PrimitiveType(Namespace.Global.Scope, "string", null);

        public override Types.Type ReturnType {
            get => Type;
        }

        public StringValueNode(Token token, string value) : base(token) {
            Value = value;
        }

        public new static ValueNode Parse(Token token) {
            if (token.Value[0] == '\"') return new StringValueNode(token, token.Value.Substring(1, token.Value.Length - 2));
            return null;
        }
    }
}