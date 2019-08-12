using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Syntax;
using BCake.Parser.Syntax.Types;
using BCake.Parser.Syntax.Scopes;
using BCake.Parser.Syntax.Expressions.Nodes.Value;
using BCake.Runtime.Nodes.Value;

namespace BCake.Runtime {
    public class Interpreter {
        public Namespace Global { get; protected set; }
        public RuntimeScope RuntimeScope { get; protected set; }

        public Interpreter(Namespace global) {
            Global = global;
            RuntimeScope = new RuntimeScope(null, Global.Scope, true);
        }

        public int Run() {
            var entrypoint = Global.Scope.GetSymbol("main", true) as BCake.Parser.Syntax.Types.FunctionType;
            if (entrypoint == null) throw new System.Exception("No entry point defined - you have to specify a global method \"int main(string[] args)\"");

            InitScopes();
            RuntimeScope.Init(Global.Scope);
            InitPrimitives();

            var exitCodeNode = new Runtime.Nodes.RuntimeFunction(entrypoint, RuntimeScope.ResolveRuntimeScope(entrypoint.Scope), new RuntimeValueNode[] { }).Evaluate() as RuntimeIntValueNode;
            var exitCode = (int)exitCodeNode.Value;

            System.Console.WriteLine($"Process ended with exit code {exitCode}");

            return exitCode;
        }

        private void InitPrimitives() {
            RuntimeScope.SetValue(
                IntValueNode.Type.Name,
                new RuntimeTypeValueNode(IntValueNode.Type)
            );
            RuntimeScope.SetValue(
                StringValueNode.Type.Name,
                new RuntimeTypeValueNode(StringValueNode.Type)
            );
        }

        private void InitScopes() {
            var root = new ScopeTreeNode(Global.Scope);
            DiscoverScopes(root);
            InitScopesByTree(root, RuntimeScope);
        }
        private void DiscoverScopes(ScopeTreeNode node) {
            if (node.Scope == null) return;

            foreach (var member in node.Scope.AllMembers) {
                switch (member.Value) {
                    case Namespace nt:
                    case ClassType ct:
                    case FunctionType ft:
                        if (member.Value.Scope == node.Scope) continue;

                        var n = new ScopeTreeNode(member.Value.Scope);
                        node.Children.Add(n);
                        DiscoverScopes(n);
                        break;
                }
            }
        }

        private void InitScopesByTree(ScopeTreeNode node, RuntimeScope parent) {
            foreach (var n in node.Children) {
                if (n.Scope == null) continue;
                InitScopesByTree(n, new RuntimeScope(parent, n.Scope));
            }
        }

        private class ScopeTreeNode {
            public List<ScopeTreeNode> Children { get; private set; }
            public Scope Scope { get; private set; }

            public ScopeTreeNode(Scope scope) {
                Scope = scope;
                Children = new List<ScopeTreeNode>();
            }
        }
    }
}