using System;
using System.Collections.Generic;
namespace crafting_interpreters {
    abstract class Stmt {
        public interface Visitor<R> {
            R visitBlockStmt(Block stmt);
            R visitExpressionStmt(Expression stmt);
            R visitPrintStmt(Print stmt);
            R visitVarStmt(Var stmt);
        }
        public abstract R accept<R>(Visitor<R> visitor);
        public class Block : Stmt {
            public readonly List<Stmt> Statements;
            public Block(List<Stmt> statements) {
                Statements = statements;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitBlockStmt(this);
            }
        }
        public class Expression : Stmt {
            public readonly Expr Expr;
            public Expression(Expr expr) {
                Expr = expr;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitExpressionStmt(this);
            }
        }
        public class Print : Stmt {
            public readonly Expr Expr;
            public Print(Expr expr) {
                Expr = expr;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitPrintStmt(this);
            }
        }
        public class Var : Stmt {
            public readonly Token Name;
            public readonly Expr Initilizer;
            public Var(Token name, Expr initilizer) {
                Name = name;
                Initilizer = initilizer;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitVarStmt(this);
            }
        }
    }
}
