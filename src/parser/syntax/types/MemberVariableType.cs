using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Exceptions;
using System.Text.RegularExpressions;

namespace BCake.Parser.Syntax.Types {
    public class MemberVariableType : Type {
        public Type Type { get; protected set; }
        public override string FullName { get { return Scope.FullName + ":" + Name; } }

        public MemberVariableType(Token token, Type parent, Access access, Type type, string name)
            : base(parent.Scope, access, name) {
            DefiningToken = token;
            Type = type;
        }
    }
}