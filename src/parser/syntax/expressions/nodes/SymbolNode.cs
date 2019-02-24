using System;
using System.Text.RegularExpressions;
using BCake.Parser.Syntax.Expressions.Nodes;

namespace BCake.Parser.Syntax.Expressions.Nodes {
    public class SymbolNode : Node, ILValue, IRValue {
        public static readonly string rxIdentifier = @"^[A-Za-z_][A-Za-z_0-9]*$";
        public Types.Type Symbol { get; protected set; }
        public override Types.Type ReturnType {
            get {
                if (Symbol is Types.LocalVariableType) return (Symbol as Types.LocalVariableType).Type;
                if (Symbol is Types.MemberVariableType) return (Symbol as Types.MemberVariableType).Type;
                if (Symbol is Types.FunctionType) return (Symbol as Types.FunctionType).ReturnType;
                if (Symbol is Types.FunctionType.ParameterType) return (Symbol as Types.FunctionType.ParameterType).Type;
                return null; // todo what now? does not make much sense
            }
        }

        public SymbolNode(Types.Type symbol) {
            Symbol = symbol;

            Console.WriteLine("New symbol node " + Symbol.FullName);
        }

        public static bool CouldBeIdentifier(string s, out Match m) {
            m = Regex.Match(s, Syntax.Expressions.Nodes.SymbolNode.rxIdentifier);
            return m.Success;
        }

        public static SymbolNode Parse(Scopes.Scope scope, Token token) {
            var symbol = scope.GetSymbol(token.Value);
            if (symbol == null) return null;
            return new SymbolNode(symbol);
        }
    }
}