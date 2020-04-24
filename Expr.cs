using System;
using System.Collections.Generic;
namespace crafting_interpreters {
    abstract class Expr {
        public interface Visitor<R> {
            R visitBinaryExpr(Binary expr);
            R visitGroupingExpr(Grouping expr);
            R visitLiteralExpr(Literal expr);
            R visitUnaryExpr(Unary expr);
        }
        public abstract R accept<R>(Visitor<R> visitor);
        public class Binary : Expr {
            public readonly Expr Left;
            public readonly Token Op;
            public readonly Expr Right;
            public Binary(Expr left, Token op, Expr right) {
                Left = left;
                Op = op;
                Right = right;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitBinaryExpr(this);
            }
        }
        public class Grouping : Expr {
            public readonly Expr Expression;
            public Grouping(Expr expression) {
                Expression = expression;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitGroupingExpr(this);
            }
        }
        public class Literal : Expr {
            public readonly Object Value;
            public Literal(Object value) {
                Value = value;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitLiteralExpr(this);
            }
        }
        public class Unary : Expr {
            public readonly Token Op;
            public readonly Expr Right;
            public Unary(Token op, Expr right) {
                Op = op;
                Right = right;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitUnaryExpr(this);
            }
        }
    }
}
