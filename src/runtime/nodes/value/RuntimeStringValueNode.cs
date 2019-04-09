using System.Linq;
using BCake.Parser.Syntax.Expressions.Nodes.Value;

namespace BCake.Runtime.Nodes.Value {
    public class RuntimeStringValueNode : RuntimeValueNode {
        public RuntimeStringValueNode(StringValueNode valueNode, RuntimeScope scope) : base(valueNode, scope) {}

        public override RuntimeValueNode OpPlus(RuntimeValueNode other) {
            return Wrap((string)Value + other.Value.ToString());
        }
        public override RuntimeValueNode OpMinus(RuntimeValueNode other) {
            var strValue = (string)Value;
            
            switch (other.Value) {
                case int i: 
                    if (i >= strValue.Length) return Wrap(strValue.Substring(0, strValue.Length - i));
                    else return Wrap("");
            }

            throw new Exceptions.RuntimeException("", DefiningToken);
        }
        public override RuntimeValueNode OpMultiply(RuntimeValueNode other) {
            var strValue = (string)Value;

            switch (other.Value) {
                case int i: {
                    var res = "";
                    if (i < 0) {
                        strValue = strValue.Reverse().ToString();
                        i = -i;
                    }

                    for (int j = 0; j < i; ++j) res += strValue;
                    return Wrap(res);
                }
            }

            throw new Exceptions.RuntimeException("", DefiningToken);
        }
        public override RuntimeValueNode OpDivide(RuntimeValueNode other) {
            var strValue = (string)Value;

            switch (other.Value) {
                case int i: {
                    if (i == 0) throw new Exceptions.RuntimeException("", DefiningToken);

                    var resultLen = strValue.Length / i;
                    strValue = strValue.Substring(resultLen);
                    if (i < 0) strValue = strValue.Reverse().ToString();
                    return Wrap(strValue);
                }
            }

            throw new Exceptions.RuntimeException("", DefiningToken);
        }

        public override RuntimeValueNode OpGreater(RuntimeValueNode other) {
            var strValue = (string)Value;

            switch (other.Value) {
                case int i:
                    return Wrap(strValue.Length > i);
                case string s:
                    return Wrap(strValue.Length > s.Length);
            }

            throw new Exceptions.RuntimeException("", DefiningToken);
        }
        public override RuntimeValueNode OpEqual(RuntimeValueNode other) {
            var strValue = (string)Value;

            switch (other.Value) {
                case int i:
                    return Wrap(strValue.Length == i);
                case string s:
                    return Wrap(strValue == s);
            }

            throw new Exceptions.RuntimeException("", DefiningToken);
        }
        public override RuntimeValueNode OpSmaller(RuntimeValueNode other) {
            var strValue = (string)Value;

            switch (other.Value) {
                case int i:
                    return Wrap(strValue.Length > i);
                case string s:
                    return Wrap(strValue.Length < s.Length);
            }

            throw new Exceptions.RuntimeException("", DefiningToken);
        }

        private RuntimeStringValueNode Wrap(string value) {
            return new RuntimeStringValueNode(
                new StringValueNode(
                    DefiningToken,
                    value
                ),
                RuntimeScope
            );
        }

        private RuntimeBoolValueNode Wrap(bool value) {
            return new RuntimeBoolValueNode(
                new BoolValueNode(
                    DefiningToken,
                    value
                ),
                RuntimeScope
            );
        }
    }
}