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
        ) : base(function.DefiningToken, scope) {
            Function = function;
            Arguments = arguments;

            RuntimeScope = scope;

            for (var i = 0; i < Function.Parameters.Length; ++i) {
                var param = Function.Parameters[i];
                var arg = arguments[i];

                if (param is FunctionType.InitializerParameterType) {
                    RuntimeScope.Parent.SetValue(param.Name, arg);
                }

                scope.SetValue(param.Name, arg);
            }
        }

        public override Nodes.Value.RuntimeValueNode Evaluate() {
            if (Function is NativeFunctionType) {
                return (Function as NativeFunctionType).Evaluate(RuntimeScope, Arguments);
            }
            else return new RuntimeScopeNode(RuntimeScope, Function.Root).Evaluate();
        }
    }
}