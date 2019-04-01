using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BCake.Parser.Exceptions;

namespace BCake.Parser.Syntax.Expressions.Nodes.Operators {
    public abstract class Operator : Node {
        private bool needsLValue, needsRValue, leftNeedsNone, rightNeedsNone, checkReturnTypes;
        OperatorAttribute.ParameterType typeLeft, typeRight;
        public Expression Left {
            get {
                return _left;
            }
            protected set {
                _left = value;
                if (value == null) return;

                if (needsLValue && !(value.Root is ILValue))
                    throw new Exceptions.InvalidArgumentException(value.DefiningToken, typeLeft);
                if (leftNeedsNone && (value.Root is ILValue || value.Root is IRValue))
                    throw new Exceptions.InvalidArgumentException(value.DefiningToken, typeLeft);

                CheckReturnTypes(value);
            }
        }
        private Expression _left;
        public Expression Right {
            get {
                return _right;
            }
            protected set {
                _right = value;
                if (value == null) return;

                if (needsRValue && !(value.Root is IRValue))
                    throw new Exceptions.InvalidArgumentException(value.DefiningToken, typeRight);
                if (rightNeedsNone && (value.Root is ILValue || value.Root is IRValue))
                    throw new Exceptions.InvalidArgumentException(value.DefiningToken, typeRight);

                CheckReturnTypes(value);
            }
        }
        private Expression _right;

        public override Types.Type ReturnType {
            // it does not matter which one we return because they have to be equal
            get => Left?.ReturnType ?? Right?.ReturnType;
        }

        // null is passed to parent class as token parameter because it is set in the parse method
        // this is the case, because the constructor has to be parameterless
        public Operator() : base(null) {
            var operatorAttr = (OperatorAttribute)this.GetType().GetCustomAttributes(typeof(OperatorAttribute), true).FirstOrDefault();
            if (operatorAttr == null) return;

            typeLeft = operatorAttr.Left;
            typeRight = operatorAttr.Right;
            needsLValue = typeLeft == OperatorAttribute.ParameterType.LValue;
            needsRValue = typeRight == OperatorAttribute.ParameterType.RValue;
            leftNeedsNone = typeLeft == OperatorAttribute.ParameterType.None;
            rightNeedsNone = typeRight == OperatorAttribute.ParameterType.None;
            checkReturnTypes = operatorAttr.CheckReturnTypes;
        }

        public virtual void OnCreated(Token token, Scopes.Scope scope) {}

        public static OperatorAttribute GetOperatorMetadata(Type t) {
            var opSymbolAttr = t.GetCustomAttributes(
                typeof(OperatorAttribute),
                true
            ).FirstOrDefault() as OperatorAttribute;
            return opSymbolAttr;
        }

        public static Node Parse(Scopes.Scope scope, Scopes.Scope typeSource, Type opType, Token token, Token[] left, Token[] right) {
            var op = (Operator)Activator.CreateInstance(opType);
            op.Left = op.ParseLeft(scope, left, typeSource);
            op.Right = op.ParseRight(scope, right, op.Left.ReturnType.Scope);
            op.DefiningToken = token;
            op.OnCreated(token, scope);
            return op;
        }

        protected virtual Expression ParseLeft(Scopes.Scope scope, Token[] tokens, Scopes.Scope typeSource) {
            return Expression.Parse(scope, tokens);
        }

        protected virtual Expression ParseRight(Scopes.Scope scope, Token[] tokens, Scopes.Scope typeSource) {
            return Expression.Parse(scope, tokens);
        }

        protected void CheckRightReturnType(Types.Type type) {
            if (Right != null && Right.ReturnType != type) throw new TypeException(Right.DefiningToken, Right.ReturnType, type);
        }
        protected void CheckReturnTypes(Expression e) {
            if (!checkReturnTypes) return;

            var other = e == Right ? Left : Right;
            if (Right == null || Left == null) return;
            if (Right.ReturnType != Left.ReturnType) throw new TypeException(e.DefiningToken, e.ReturnType, other.ReturnType);
        }
    }
}