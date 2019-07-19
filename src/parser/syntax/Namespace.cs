using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Exceptions;
using BCake.Parser.Syntax.Types;
using BCake.Parser.Syntax.Expressions.Nodes;
using BCake.Parser.Syntax.Expressions.Nodes.Value;
using BCake.Parser.Syntax.Types.Native.Std;

namespace BCake.Parser.Syntax {
    public class Namespace : Types.ComplexType {
        public static Namespace Global { get; protected set; }

        public Namespace() : base(null, "public") {
            Scope = new Scopes.Scope(null, this);
            Global = this;

            InitPrimitives();
            InitNativeFunctions();
        }
        public Namespace(Scopes.Scope parent, Access access, string name, BCake.Parser.Token[] tokens)
            : base(parent, name, access) {
            Access = access;
            Name = name;
            this.tokens = tokens;

            Scope = new Scopes.Scope(parent, this);
        }

        public override void ParseInner() {
            Parser.ParseTypes(Scope, tokens, new string[] { "class", "function" });
        }

        private void InitPrimitives() {
            Scope.Declare(
                StringValueNode.Type,
                IntValueNode.Type,
                BoolValueNode.Type
            );

            IntValueNode.Type.Scope.Declare(IntToStringCast.Implementation);
        }

        private void InitNativeFunctions() {
            Scope.Declare(
                Print.Implementation,
                Println.Implementation
            );

            StringValueNode.Type.Scope.Declare(
                BCake.Parser.Syntax.Types.Native.String.Operators.StringOperatorPlus.Implementation
            );

            IntValueNode.Type.Scope.Declare(
                BCake.Parser.Syntax.Types.Native.Integer.Operators.IntegerOperatorPlus.Implementation
            );
        }
    }
}