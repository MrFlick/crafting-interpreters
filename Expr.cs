using System;
using System.Collections.Generic;
namespace crafting_interpreters {
    abstract class Expr {
        public interface Visitor<R> {
            R visitAssignExpr(Assign expr);
            R visitBinaryExpr(Binary expr);
            R visitCallExpr(Call expr);
            R visitTernaryExpr(Ternary expr);
            R visitGroupingExpr(Grouping expr);
            R visitLiteralExpr(Literal expr);
            R visitLogicalExpr(Logical expr);
            R visitUnaryExpr(Unary expr);
            R visitVariableExpr(Variable expr);
        }
        public abstract R accept<R>(Visitor<R> visitor);
        public class Assign : Expr {
            public readonly Token Name;
            public readonly Expr Value;
            public Assign(Token name, Expr value) {
                Name = name;
                Value = value;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitAssignExpr(this);
            }
        }
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
        public class Call : Expr {
            public readonly Expr Callee;
            public readonly Token Paren;
            public readonly List<Expr> Arguments;
            public Call(Expr callee, Token paren, List<Expr> arguments) {
                Callee = callee;
                Paren = paren;
                Arguments = arguments;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitCallExpr(this);
            }
        }
        public class Ternary : Expr {
            public readonly Expr Cond;
            public readonly Expr IfTrue;
            public readonly Expr IfFalse;
            public Ternary(Expr cond, Expr ifTrue, Expr ifFalse) {
                Cond = cond;
                IfTrue = ifTrue;
                IfFalse = ifFalse;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitTernaryExpr(this);
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
        public class Logical : Expr {
            public readonly Expr Left;
            public readonly Token Op;
            public readonly Expr Right;
            public Logical(Expr left, Token op, Expr right) {
                Left = left;
                Op = op;
                Right = right;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitLogicalExpr(this);
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
        public class Variable : Expr {
            public readonly Token Name;
            public Variable(Token name) {
                Name = name;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitVariableExpr(this);
            }
        }
    }
}
