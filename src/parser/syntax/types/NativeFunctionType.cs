using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BCake.Parser.Exceptions;
using BCake.Parser.Syntax.Expressions.Nodes;

using BCake.Runtime.Nodes.Value;

namespace BCake.Parser.Syntax.Types {
    public abstract class NativeFunctionType : FunctionType {
        public NativeFunctionType(Type returnType, string name, ParameterType[] parameters)
            : base(returnType, name, parameters) {
        }

        public override void ParseInner() {}

        public abstract RuntimeValueNode Evaluate(RuntimeValueNode[] arguments);
    }
}