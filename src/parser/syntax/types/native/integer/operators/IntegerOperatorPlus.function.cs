using BCake.Runtime;
using BCake.Runtime.Nodes.Value;
using BCake.Parser.Syntax.Expressions.Nodes.Value;

namespace BCake.Parser.Syntax.Types.Native.Integer.Operators {
    public class IntegerOperatorPlus : NativeFunctionType {
        public static NativeFunctionType Implementation = new IntegerOperatorPlus();

        private IntegerOperatorPlus() : base(
            IntValueNode.Type.Scope,
            IntValueNode.Type,
            "!operator_plus",
            new ParameterType[] { 
                new ParameterType(null, IntValueNode.Type, "other")
             }
        ) {
            Scope.Declare(
                new ParameterType(DefiningToken, IntValueNode.Type, "other")
            );
        }

        public override RuntimeValueNode Evaluate(RuntimeScope scope, RuntimeValueNode[] arguments) {
            var left = (int)arguments[0].Value;
            var right = (int)arguments[1].Value;

            return new RuntimeIntValueNode(
                new Expressions.Nodes.Value.IntValueNode(
                    DefiningToken,
                    left + right
                ),
                null
            );
        }
    }
}