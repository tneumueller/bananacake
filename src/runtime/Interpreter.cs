using BCake.Parser.Syntax;
using BCake.Parser.Syntax.Expressions.Nodes.Value;
using BCake.Runtime.Nodes.Value;

namespace BCake.Runtime {
    public class Interpreter {
        public Namespace Global { get; protected set; }
        public RuntimeScope RuntimeScope { get; protected set; }

        public Interpreter(Namespace global) {
            Global = global;
            RuntimeScope = new RuntimeScope(null, global.Scope);
        }

        public int Run() {
            var entrypoint = Global.Scope.GetSymbol("main", true) as BCake.Parser.Syntax.Types.FunctionType;
            if (entrypoint == null) throw new System.Exception("No entry point defined - you have to specify a global method \"int main(string[] args)\"");

            var exitCodeNode = new Runtime.Nodes.RuntimeFunction(entrypoint, RuntimeScope, new RuntimeValueNode[] {}).Evaluate() as RuntimeIntValueNode;
            var exitCode = (int)exitCodeNode.Value;

            System.Console.WriteLine($"Process ended with exit code {exitCode}");

            return exitCode;
        }
    }
}