using BCake.Runtime;
using BCake.Runtime.Nodes.Value;
using BCake.Parser.Syntax.Expressions.Nodes.Value;

namespace BCake.Parser.Syntax.Types.Native.Std {
    public class IntToStringCast : NativeFunctionType {
        public static NativeFunctionType Implementation = new IntToStringCast();

        private IntToStringCast() : base(
            IntValueNode.Type.Scope,
            null,
            "!as_string",
            new ParameterType[] {}
        ) {}

        public override RuntimeValueNode Evaluate(RuntimeScope scope, RuntimeValueNode[] arguments) {
            return new RuntimeStringValueNode(
                new Expressions.Nodes.Value.StringValueNode(
                    DefiningToken,
                    ((int)arguments[0].Value).ToString()
                ),
                null
            );
        }
    }
}