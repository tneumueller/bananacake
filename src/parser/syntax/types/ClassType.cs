using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BCake.Parser.Exceptions;
using BCake.Parser.Syntax.Expressions.Nodes;

namespace BCake.Parser.Syntax.Types {
    public class ClassType : ComplexType {
        public override string FullName { get => Scope.FullName; }
        public ClassType(Scopes.Scope parent, Access access, string name, BCake.Parser.Token[] tokens)
            : base(null, name) {
            Access = access;
            this.tokens = tokens;

            Scope = new Scopes.Scope(parent, this);

            var thisMember = new MemberVariableType(
                DefiningToken,
                this,
                Access.@private,
                this,
                "this"
            );
            Scope.Declare(thisMember, "this");
        }

        public override void ParseInner() {
            Parser.ParseTypes(Scope, tokens, new string[] { "function", "cast", "variable" });
        }
    }
}