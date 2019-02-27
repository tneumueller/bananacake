using System;
using System.Text.RegularExpressions;
using BCake.Parser.Syntax.Types;

namespace BCake.Parser.Syntax.Expressions.Nodes.Value {
    public class IntValueNode : ValueNode {
        public static readonly string rxOctIntLiteral = @"^0([0-7]*)$";
        public static readonly string rxDecIntLiteral = @"^([0-9]+)$";
        public static readonly string rxHexIntLiteral = @"^0x([0-9a-fA-F]+)$";
        public static readonly string rxBinIntLiteral = @"^0b([0-1]+)$";
        public static Types.PrimitiveType Type = new PrimitiveType(Namespace.Global.Scope, "int");

        public override Types.Type ReturnType {
            get => Type;
        }

        public IntValueNode(Token token, int value) : base(token) {
            Value = value;
            Console.WriteLine("New IntValueNode with value " + Value);
        }

        public new static ValueNode Parse(Token token) {
            Match m;

            if ((m = Regex.Match(token.Value, rxOctIntLiteral)).Success) {
                return new IntValueNode(token, Convert.ToInt32(m.Groups[0].Value, 8));
            } else if ((m = Regex.Match(token.Value, rxDecIntLiteral)).Success) {
                return new IntValueNode(token, Convert.ToInt32(m.Groups[1].Value, 10));
            } else if ((m = Regex.Match(token.Value, rxHexIntLiteral)).Success) {
                return new IntValueNode(token, Convert.ToInt32(m.Groups[1].Value, 16));
            } else if ((m = Regex.Match(token.Value, rxBinIntLiteral)).Success) {
                return new IntValueNode(token, Convert.ToInt32(m.Groups[1].Value, 2));
            }

            return null;
        }
    }
}