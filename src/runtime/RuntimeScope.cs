using System.Collections.Generic;
using BCake.Parser.Syntax;
using BCake.Parser.Syntax.Expressions.Nodes;
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

            foreach (var member in scope.AllMembers) InitDefault(member);
        }

        private void InitDefault(KeyValuePair<string, Type> member) {
            RuntimeValueNode defaultValue = null;

            switch (member.Value) {
                case PrimitiveType t: break;
                case FunctionType t: break;
                case FunctionType.ParameterType t: break;
                case Namespace t: break;
                case ComplexType t: defaultValue = new RuntimeNullValueNode(new Parser.Syntax.Expressions.Nodes.Value.NullValueNode(new Parser.Token()), this); break;
                case LocalVariableType t:
                    t.Type
                    break;
                default: defaultValue = RuntimeValueNode.InstantiateWithDefaultValue(member.Value, this); break;
            }

            Values.Add(member.Key, defaultValue);
        }

        public RuntimeValueNode GetValue(string symbol) {
            if (!Values.ContainsKey(symbol)) return Parent.GetValue(symbol);
            // we don't need to check because an invalid access should fail at parse time
            return Values[symbol];
        }

        public RuntimeValueNode GetValueHere(string symbol) {
            // we don't need to check because an invalid access should fail at parse time
            return Values[symbol];
        }

        public bool SetValue(string symbol, RuntimeValueNode value) {
            if (!Values.ContainsKey(symbol)) return Parent?.SetValue(symbol, value) == true;

            Values[symbol] = value;
            return true;
        }

        public static RuntimeScope ResolveRuntimeScope(Scope scope) {
            if (!RuntimeScopeForScope.ContainsKey(scope)) {
                if (scope?.Parent == null) throw new Exceptions.RuntimeException("Could not resolve runtime scope", new Parser.Token());
                return ResolveRuntimeScope(scope.Parent);
            }
            return RuntimeScopeForScope[scope];
        }

        public static SymbolNode ResolveSymbolNode(SymbolNode n) {
            if (n.Symbol is CompositeType) return ResolveCompositeType(n.Symbol as CompositeType);
            else return n;
        }

        private static SymbolNode ResolveCompositeType(CompositeType t) {
            var symbol = t.OperatorAccess.SymbolToAccess;
            var scope = ResolveRuntimeScope(symbol.Scope);

            var symbolValue = scope.GetValue(symbol.Name);
            if (symbolValue == null) throw new Exceptions.NullReferenceException(symbol, t.OperatorAccess.DefiningToken);

            if (!(symbolValue is RuntimeClassInstanceValueNode)) {
                throw new Exceptions.RuntimeException("Cannot access properties on non-class types", t.OperatorAccess.DefiningToken);
            }

            var instance = symbolValue as RuntimeClassInstanceValueNode;
            // var result = instance.AccessMember(t.OperatorAccess.Right.Root)

            // return new SymbolNode(
            //     actualType.DefiningToken,
            //     actualType
            // );
            return null;
        }
    }
}