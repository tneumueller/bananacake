using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Exceptions;
using System.Text.RegularExpressions;

namespace BCake.Parser.Syntax.Types {
    public class MemberVariableType : Type {
        public ClassType Class { get; protected set; }
        public string Type { get; protected set; }
        public override string FullName { get { return Class.FullName + ":" + Name; } }

        public MemberVariableType(Token token, Namespace ns, ClassType c, string access, string type, string name) {
            DefiningToken = token;
            Namespace = ns;
            Class = c;
            Access = access;
            Type = type;
            Name = name;

            Console.WriteLine($"New variable {Access} {Type} {FullName}");
        }
    }
}