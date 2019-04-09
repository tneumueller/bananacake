using System;
using System.Text.RegularExpressions;
using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Syntax.Expressions.Nodes.Value {
    public class BoolValueNode : ValueNode {
        public static Types.PrimitiveType Type = new PrimitiveType(Namespace.Global.Scope, "bool", false);

        public override Types.Type ReturnType {
            get => Type;
        }

        public BoolValueNode(Token token, bool value) : base(token) {
            Value = value;
        }

        public new static ValueNode Parse(Token token) {
            if (token.Value == "true") return new BoolValueNode(token, true);
            if (token.Value == "false") return new BoolValueNode(token, false);
            return null;
        }
    }
}