using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Syntax.Types;
using BCake.Parser.Exceptions;

namespace BCake.Parser.Syntax.Scopes {
    public class Scope {
        public static int ScopeCount { get; protected set; }
        public int Id { get; protected set; }
        public Type ParentType { get; protected set; }
        public string FullName {
            get {
                if (ParentType == null) return $"Scope #{Id}";
                else return ParentType.FullName;
            }
        }
        private Dictionary<string, Type> MembersByName = new Dictionary<string, Type>();

        public Scope() {
            Id = ScopeCount++;
        }

        public Scope(Type parent) {
            Id = ScopeCount++;
            ParentType = parent;
        }

        public void RegisterMember(Type m) {
            if (MembersByName.ContainsKey(m.Name)) throw new DuplicateDeclarationException(m.DefiningToken, MembersByName[m.Name]);
            MembersByName.Add(m.Name, m);
        }
    }
}