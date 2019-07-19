using BCake.Runtime;
using BCake.Runtime.Nodes.Value;
using BCake.Parser.Syntax.Expressions.Nodes.Value;

namespace BCake.Parser.Syntax.Types.Native.String.Operators {
    public class StringOperatorPlus : NativeFunctionType {
        public static NativeFunctionType Implementation = new StringOperatorPlus();

        private StringOperatorPlus() : base(
            StringValueNode.Type.Scope,
            StringValueNode.Type,
            "!operator_plus",
            new ParameterType[] { 
                new ParameterType(null, StringValueNode.Type, "other")
             }
        ) {
            Scope.Declare(
                new ParameterType(DefiningToken, StringValueNode.Type, "other")
            );
        }

        public override RuntimeValueNode Evaluate(RuntimeScope scope, RuntimeValueNode[] arguments) {
            var left = (string)arguments[0].Value;
            var right = (string)arguments[1].Value;

            return new RuntimeStringValueNode(
                new Expressions.Nodes.Value.StringValueNode(
                    DefiningToken,
                    left + right
                ),
                scope
            );
        }
    }
}