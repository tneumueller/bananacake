using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BCake.Parser.Exceptions;
using BCake.Parser.Syntax.Expressions.Nodes;

using BCake.Runtime;
using BCake.Runtime.Nodes.Value;

namespace BCake.Parser.Syntax.Types {
    public abstract class NativeFunctionType : FunctionType {
        public NativeFunctionType(Scopes.Scope scope, Type returnType, string name, ParameterType[] parameters)
            : base(scope, returnType, name, parameters) {
        }

        public NativeFunctionType(Scopes.Scope scope, Type returnType, string name, ParameterType[] parameters, NativeFunctionType[] overloads)
            : base(scope, returnType, name, parameters) {
            Overloads = overloads;
        }

        public override void ParseInner() {}

        public abstract RuntimeValueNode Evaluate(RuntimeScope scope, RuntimeValueNode[] arguments);
    }
}