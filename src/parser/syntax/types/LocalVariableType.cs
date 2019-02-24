using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Exceptions;
using System.Text.RegularExpressions;

namespace BCake.Parser.Syntax.Types {
    public class LocalVariableType : Type {
        public Type Type { get; protected set; }
        public override string FullName { get { return Scope.FullName + ":" + Name; } }

        public LocalVariableType(Token token, Scopes.Scope scope, Type type, string name) {
            DefiningToken = token;
            Type = type;
            Name = name;
            Scope = scope;

            Console.WriteLine($"New local variable {Type.FullName} {FullName}");
        }
    }
}