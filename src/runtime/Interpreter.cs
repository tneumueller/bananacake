using BCake.Parser.Syntax;

namespace BCake.Runtime {
    public class Interpreter {
        public Namespace Global { get; protected set; }

        public Interpreter(Namespace global) {
            Global = global;
        }

        public void Run() {
            var entrypoint = Global.Scope.GetSymbol("main", true);
            if (entrypoint == null) throw new System.Exception("No entry point defined - you have to specify a global method \"int main(string[] args)\"");
        }
    }
}