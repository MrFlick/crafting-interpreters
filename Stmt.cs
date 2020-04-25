using System;
using System.Collections.Generic;
namespace crafting_interpreters {
    abstract class Stmt {
        public interface Visitor<R> {
            R visitBlockStmt(Block stmt);
            R visitExpressionStmt(Expression stmt);
            R visitFunctionStmt(Function stmt);
            R visitIfStmt(If stmt);
            R visitPrintStmt(Print stmt);
            R visitReturnStmt(Return stmt);
            R visitVarStmt(Var stmt);
            R visitWhileStmt(While stmt);
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
        public class Function : Stmt {
            public readonly Token Name;
            public readonly List<Token> Parameters;
            public readonly List<Stmt> Body;
            public Function(Token name, List<Token> parameters, List<Stmt> body) {
                Name = name;
                Parameters = parameters;
                Body = body;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitFunctionStmt(this);
            }
        }
        public class If : Stmt {
            public readonly Expr Cond;
            public readonly Stmt ThenBranch;
            public readonly Stmt ElseBranch;
            public If(Expr cond, Stmt thenBranch, Stmt elseBranch) {
                Cond = cond;
                ThenBranch = thenBranch;
                ElseBranch = elseBranch;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitIfStmt(this);
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
        public class Return : Stmt {
            public readonly Token Keyword;
            public readonly Expr Value;
            public Return(Token keyword, Expr value) {
                Keyword = keyword;
                Value = value;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitReturnStmt(this);
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
        public class While : Stmt {
            public readonly Expr Cond;
            public readonly Stmt Body;
            public While(Expr cond, Stmt body) {
                Cond = cond;
                Body = body;
            }
            public override R accept<R>(Visitor<R> visitor) {
                return visitor.visitWhileStmt(this);
            }
        }
    }
}
