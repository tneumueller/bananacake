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
        public ClassType(Scopes.Scope parent, string access, string name, BCake.Parser.Token[] tokens) {
            Access = access;
            Name = name;
            this.tokens = tokens;

            Scope = new Scopes.Scope(parent, this);
            Scope.Declare(this, "this");
        }

        public override void ParseInner() {
            Parser.ParseTypes(Scope, tokens, new string[] { "function", "variable" });
        }
    }
}