using System;
using System.Text;

namespace crafting_interpreters
{
    class AstPrinter : Expr.Visitor<String> {
        public String print(Expr expr) {
            return expr.accept(this);
        }
        public string visitBinaryExpr(Expr.Binary expr) {
            return parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
        }
        public string visitLogicalExpr(Expr.Logical expr)
        {
            return parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
        }
        public string visitTernaryExpr(Expr.Ternary expr) {
            return parenthesize("ternary", expr.Cond, expr.IfTrue, expr.IfFalse);
        }
        public string visitGroupingExpr(Expr.Grouping expr) {
            return parenthesize("group", expr.Expression);
        }
        public string visitLiteralExpr(Expr.Literal expr) {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }
        public string visitUnaryExpr(Expr.Unary expr) {
            return parenthesize(expr.Op.Lexeme, expr.Right);
        }
        public string visitVariableExpr(Expr.Variable expr)
        {
            return parenthesize("var", expr);
        }
        public string visitAssignExpr(Expr.Assign expr)
        {
            return parenthesize("assing", expr, expr.Value);
        }

        private string parenthesize(string name, params Expr[] exprs) {
            StringBuilder builder = new StringBuilder();

            builder.Append("(").Append(name);
            foreach (Expr expr in exprs) {
                builder.Append(" ");
                builder.Append(expr.accept(this));
            }
            builder.Append(")");

            return builder.ToString();
        }


    }
}
