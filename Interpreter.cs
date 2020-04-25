namespace crafting_interpreters
{
    class Interpreter : Expr.Visitor<object>
    {
        public void interpret(Expr expression) {
            try {
                object value = evaluate(expression);
                System.Console.WriteLine(stringify(value));
            } catch (RuntimeError error) {
                Lox.runtimeError(error);
            }
        }
        public object visitBinaryExpr(Expr.Binary expr)
        {
            object left = evaluate(expr.Left);
            object right = evaluate(expr.Right);

            switch (expr.Op.Type) {
                case TokenType.MINUS:
                    checkNumberOperands(expr.Op, left, right);
                    return (double)left - (double)right;
                case TokenType.SLASH:
                    checkNumberOperands(expr.Op, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    checkNumberOperands(expr.Op, left, right);
                    return (double)left * (double)right;
                case TokenType.PLUS:
                    if (left is double && right is double) {
                        return (double)left + (double)right;
                    }
                    if (left is string && right is string) {
                        return (string)left + (string)right;
                    }
                    throw new RuntimeError(expr.Op,
                        "Operands must be two numbers or strings");
                case TokenType.GREATER:
                    checkNumberOperands(expr.Op, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    checkNumberOperands(expr.Op, left, right); 
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    checkNumberOperands(expr.Op, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    checkNumberOperands(expr.Op, left, right);
                    return (double)left <= (double)right;
                case TokenType.BANG_EQUAL:
                    return !isEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return isEqual(left, right);
            }

            return null;
        }

        public object visitGroupingExpr(Expr.Grouping expr)
        {
            return evaluate(expr.Expression);
        }

        public object visitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object visitTernaryExpr(Expr.Ternary expr)
        {
            bool cond = isTruthy(evaluate(expr.Cond));
            if (cond) {
                return evaluate(expr.IfTrue);
            } else {
                return evaluate(expr.IfFalse);
            }
        }

        public object visitUnaryExpr(Expr.Unary expr)
        {
            object right = evaluate(expr.Right);
            switch (expr.Op.Type) {
                case TokenType.MINUS:
                    return -(double)right;
                case TokenType.BANG:
                    return !isTruthy(right);
            }
            return null;
        }

        private void checkNumberOperator(Token op, object operand) {
            if(operand is double) return;
            throw new RuntimeError(op, "Operand must be a number.");
        }

        private void checkNumberOperands(Token op, object left, object right) {
            if(left is double && right is double) return;
            throw new RuntimeError(op, "Operands must be a number.");
        }        

        private bool isTruthy(object obj) {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        private bool isEqual(object a, object b) {
            if (a==null & b==null) return true;
            if (a==null) return false;
            return a.Equals(b);
        }

        private string stringify(object obj) {
            if (obj == null) return "nil";
            return obj.ToString();
        }

        private object evaluate(Expr expr) {
            return expr.accept(this);
        }
    }
}
