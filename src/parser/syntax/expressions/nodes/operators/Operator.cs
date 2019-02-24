using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Exceptions;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    public abstract class Operator : Node {
        private bool needsLValue, needsRValue, leftNeedsNone, rightNeedsNone;
        OperatorSymbolAttribute.OperatorParameterType typeLeft, typeRight;
        public Expression Left {
            get {
                return _left;
            }
            protected set {
                if (needsLValue && !(value.Root is ILValue))
                    throw new Exceptions.InvalidArgumentException(value.DefiningToken, typeLeft);
                if (leftNeedsNone && (value.Root is ILValue || value.Root is IRValue))
                    throw new Exceptions.InvalidArgumentException(value.DefiningToken, typeLeft);

                _left = value;
                CheckReturnTypes(value);
            }
        }
        private Expression _left;
        public Expression Right {
            get {
                return _right;
            }
            protected set {
                if (needsRValue && !(value.Root is IRValue))
                    throw new Exceptions.InvalidArgumentException(value.DefiningToken, typeRight);
                if (rightNeedsNone && (value.Root is ILValue || value.Root is IRValue))
                    throw new Exceptions.InvalidArgumentException(value.DefiningToken, typeRight);

                _right = value;
                CheckReturnTypes(value);
            }
        }
        private Expression _right;

        public override Types.Type ReturnType {
            // it does not matter which one we return because they have to be equal
            get => Left.ReturnType;
        }

        public Operator() {
            var operatorAttr = (OperatorSymbolAttribute)this.GetType().GetCustomAttributes(typeof(OperatorSymbolAttribute), true).FirstOrDefault();
            if (operatorAttr == null) return;

            typeLeft = operatorAttr.Left;
            typeRight = operatorAttr.Right;
            needsLValue = typeLeft == OperatorSymbolAttribute.OperatorParameterType.LValue;
            needsRValue = typeRight == OperatorSymbolAttribute.OperatorParameterType.RValue;
            leftNeedsNone = typeLeft == OperatorSymbolAttribute.OperatorParameterType.None;
            rightNeedsNone = typeRight == OperatorSymbolAttribute.OperatorParameterType.None;
        }

        public static string GetOperatorSymbol(Type t) {
            var opSymbolAttr = t.GetCustomAttributes(
                typeof(OperatorSymbolAttribute),
                true
            ).FirstOrDefault() as OperatorSymbolAttribute;

            if (opSymbolAttr == null) return null;
            return opSymbolAttr.Symbol;
        }

        public static Node Parse(Scopes.Scope scope, Type opType, Token[] left, Token[] right) {
            var op = (Operator)Activator.CreateInstance(opType);
            
            Console.Write("Left: ");
            foreach (var t in left) Console.Write(t.Value);
            Console.WriteLine();

            Console.Write("Right: ");
            foreach (var t in right) Console.Write(t.Value);
            Console.WriteLine();

            op.Left = Expression.Parse(scope, left);
            op.Right = Expression.Parse(scope, right);

            return op;
        }

        protected void CheckRightReturnType(Types.Type type) {
            if (Right != null && Right.ReturnType != type) throw new TypeException(Right.DefiningToken, Right.ReturnType, type);
        }
        protected void CheckReturnTypes(Expression e) {
            var other = e == Right ? Left : Right;
            if (Right == null || Left == null) return;
            if (Right.ReturnType != Left.ReturnType) throw new TypeException(e.DefiningToken, e.ReturnType, other.ReturnType);
        }
    }
}