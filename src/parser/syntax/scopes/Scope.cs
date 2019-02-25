using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Syntax.Types;
using BCake.Parser.Exceptions;

namespace BCake.Parser.Syntax.Scopes {
    public class Scope {
        public static int ScopeCount { get; protected set; }
        public int Id { get; protected set; }
        public Type Type { get; protected set; }
        public Scope Parent { get; protected set; }
        public string FullName {
            get {
                if (Id == 0) return null;
                return Parent.FullName + (Type?.Name != null ?  "." + Type?.Name : null);
            }
        }
        private Dictionary<string, Type> MembersByName = new Dictionary<string, Type>();
        public IEnumerable<Type> AllMembers {
            get {
                return MembersByName.Values;
            }
        }

        public Scope() {
            Id = ScopeCount++;
        }
        public Scope(Scope parent) {
            Id = ScopeCount++;
            Parent = parent;
        }
        public Scope(Scope parent, Type type) {
            Id = ScopeCount++;
            Parent = parent;
            Type = type;
        }

        public void Declare(params Type[] members) {
            foreach (var m in members) Declare(m);
        }
        public void Declare(Type m) {
            Declare(m, m.Name);
        }
        public void Declare(Type m, string nameOverride) {
            if (MembersByName.ContainsKey(nameOverride)) throw new DuplicateDeclarationException(m.DefiningToken, MembersByName[nameOverride]);
            MembersByName.Add(nameOverride, m);
        }

        public Type GetSymbol(string name, bool localOnly = false) {
            if (!MembersByName.ContainsKey(name)) {
                if (!localOnly) return Parent?.GetSymbol(name);
                else return null;
            }
            return MembersByName[name]; 
        }

        public FunctionType GetClosestFunction() {
            if (Type is FunctionType) return Type as FunctionType;
            else return Parent?.GetClosestFunction();
        }
    }
}