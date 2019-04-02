using System;

namespace BCake.Runtime.Nodes.Value {
    public class RuntimeValueNodeAttribute : Attribute {
        public object Value;
        public Type ValueNodeType;
    }
}