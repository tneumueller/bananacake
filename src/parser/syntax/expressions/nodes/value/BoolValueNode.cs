using System;
using System.Text.RegularExpressions;
using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Syntax.Expressions.Nodes.Value {
    public class BoolValueNode : ValueNode {
        public static Types.PrimitiveType Type = new PrimitiveType(Namespace.Global.Scope, "bool");

        public override Types.Type ReturnType {
            get => Type;
        }

        public BoolValueNode(bool value) {
            Value = value;
            Console.WriteLine("New BoolValueNode with value " + Value);
        }

        public new static ValueNode Parse(Token token) {
            if (token.Value == "true") return new BoolValueNode(true);
            if (token.Value == "false") return new BoolValueNode(false);
            return null;
        }
    }
}