using System.Collections.Generic;
using BCake.Parser.Syntax.Scopes;
using BCake.Parser.Syntax.Types;
using BCake.Runtime.Nodes.Value;

namespace BCake.Runtime {
    public class RuntimeScope {
        public RuntimeScope Parent { get; protected set; }
        public Scope Scope { get; protected set; }

        protected Dictionary<string, RuntimeValueNode> Values = new Dictionary<string, RuntimeValueNode>();
        protected static Dictionary<Scope, RuntimeScope> RuntimeScopeForScope = new Dictionary<Scope, RuntimeScope>();

        public RuntimeScope(RuntimeScope parent, Scope scope) {
            Parent = parent;
            Scope = scope;

            if (!RuntimeScopeForScope.ContainsKey(scope))
                RuntimeScopeForScope.Add(scope, this);

            foreach (var member in Scope.AllMembers) {
                Values.Add(member.Name, null);
            }
        }

        public RuntimeValueNode GetValue(string symbol) {
            if (!Values.ContainsKey(symbol)) return Parent.GetValue(symbol);
            return Values[symbol];
        }

        public void SetValue(string symbol, RuntimeValueNode value) {
            Values[symbol] = value;
        }

        public static RuntimeScope ResolveRuntimeScope(Scope scope) {
            if (!RuntimeScopeForScope.ContainsKey(scope)) {
                if (scope?.Parent == null) throw new Exceptions.RuntimeException("Could not resolve runtime scope", new Parser.Token());
                return ResolveRuntimeScope(scope.Parent);
            }
            return RuntimeScopeForScope[scope];
        }
    }
}