using BCake.Parser.Syntax.Types;
using BCake.Runtime.Nodes.Value;

namespace BCake.Runtime.Nodes {
    public class RuntimeFunction : RuntimeNode {
        public FunctionType Function { get; protected set; }
        public RuntimeValueNode[] Arguments { get; protected set; }

        public RuntimeFunction(
            FunctionType function,
            RuntimeScope scope,
            RuntimeValueNode[] arguments
        )
            : base(function.DefiningToken, scope) {
            Function = function;
            Arguments = arguments;

            RuntimeScope = scope = new RuntimeScope(
                scope,
                function.Scope
            );

            for (var i = 0; i < Function.Parameters.Length; ++i) {
                scope.SetValue(
                    Function.Parameters[i].Name,
                    arguments[i]
                );
            }
        }

        public override Nodes.Value.RuntimeValueNode Evaluate() {
            if (Function is NativeFunctionType) return (Function as NativeFunctionType).Evaluate(Arguments);
            else return new RuntimeScopeNode(RuntimeScope, Function.Root).Evaluate();
        }
    }
}