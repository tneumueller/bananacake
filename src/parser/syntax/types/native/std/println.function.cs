using BCake.Parser.Syntax.Expressions.Nodes.Value;

using BCake.Runtime;
using BCake.Runtime.Nodes.Value;

namespace BCake.Parser.Syntax.Types.Native.Std {
    public class Println : NativeFunctionType {
        public static NativeFunctionType Implementation = new Println(StringValueNode.Type, true);

        private Println(Type paramType, bool initOverloads = false) : base(
            Namespace.Global.Scope,
            null,
            "println",
            new ParameterType[] {
                 new ParameterType(null, paramType, "s")
            },
            initOverloads ? new Println[] {
                new Println(IntValueNode.Type),
                new Println(BoolValueNode.Type)
            } : null
        ) { }

        public override RuntimeValueNode Evaluate(RuntimeScope scope, RuntimeValueNode[] arguments) {
            System.Console.WriteLine(arguments[0].Value);

            return null;
        }
    }
}