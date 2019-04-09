using BCake.Parser.Syntax.Expressions.Nodes.Value;

using BCake.Runtime.Nodes.Value;

namespace BCake.Parser.Syntax.Types.Native.Std {
    public class Print : NativeFunctionType {
        public static NativeFunctionType Implementation = new Print();

        private Print() : base(
            null,
            "print",
            new ParameterType[] {
                 new ParameterType(null, StringValueNode.Type, "s")
            }
        ) {}

        public override RuntimeValueNode Evaluate(RuntimeValueNode[] arguments) {
            System.Console.Write(arguments[0].Value);

            return null;
        }
    }
}