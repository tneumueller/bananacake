using BCake.Runtime.Nodes.Value;

namespace BCake.Parser.Syntax.Types.Native.Std {
    public class IntToStringCast : NativeFunctionType {
        public static NativeFunctionType Implementation = new IntToStringCast();

        private IntToStringCast() : base(
            null,
            "!as_string",
            new ParameterType[] {}
        ) {}

        public override RuntimeValueNode Evaluate(RuntimeValueNode[] arguments) {
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