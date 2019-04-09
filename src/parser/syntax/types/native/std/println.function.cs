using BCake.Parser.Syntax.Expressions.Nodes.Value;

using BCake.Runtime.Nodes.Value;

namespace BCake.Parser.Syntax.Types.Native.Std {
    public class Println : NativeFunctionType {
        public static NativeFunctionType Implementation = new Println();

        private Println() : base(
            null,
            "println",
            new ParameterType[] {
                 new ParameterType(null, IntValueNode.Type, "i")
            }
        ) {}

        public override RuntimeValueNode Evaluate(RuntimeValueNode[] arguments) {
            System.Console.WriteLine(arguments[0].Value);

            return null;
        }
    }
}