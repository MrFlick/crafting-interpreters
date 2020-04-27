using System;
using System.Collections.Generic;

namespace crafting_interpreters
{
    class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
    {
        public Envir globals = new Envir();
        private Envir env;
        private readonly Dictionary<Expr, int> locals = new Dictionary<Expr, int>();

        public Interpreter()
        {
            env = globals;
            globals.define("clock", new Functions.ClockFunction());
        }

        public void interpret(List<Stmt> statements) {
            try {
                foreach (Stmt s in statements) {
                    execute(s);
                }
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

        public object visitCallExpr(Expr.Call expr)
        {
            object callee = evaluate(expr.Callee);
            List<object> args = expr.Arguments.ConvertAll(
                new Converter<Expr, object>(x => evaluate(x))
            );
            if(!(callee is LoxCallable)) {
                throw new RuntimeError(expr.Paren, 
                    "Can only call functions and classes");
            }
            LoxCallable function = (LoxCallable)callee;
            if (args.Count != function.arity()) {
                throw new RuntimeError(expr.Paren, 
                    $"Expected {function.arity()} but got {args.Count}");

            }
            return function.call(this, args);
        }

        public object visitGetExpr(Expr.Get expr) {
            Object instance = evaluate(expr.Instance);
            if(instance is LoxInstance) {
                return ((LoxInstance) instance).get(expr.Name);
            }

            throw new RuntimeError(expr.Name, 
                "Only instances have properties");
        }

        public object visitGroupingExpr(Expr.Grouping expr)
        {
            return evaluate(expr.Expression);
        }

        public object visitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object visitLogicalExpr(Expr.Logical expr)
        {
            Object left = evaluate(expr.Left);

            if (expr.Op.Type == TokenType.OR) {
                if (isTruthy(left)) return left;
            } else {
                if (!isTruthy(left)) return left;
            }
            
            return evaluate(expr.Right);
        }

        public object visitSetExpr(Expr.Set expr) 
        {
            object instance = evaluate(expr.Instance);

            if(!(instance is LoxInstance)) {
                throw new RuntimeError(expr.Name, "Only instances have fields");
            }

            object value = evaluate(expr.Value);
            ((LoxInstance)instance).set(expr.Name, value);
            return null;
        }

        public object visitThisExpr(Expr.This expr) {
            return lookUpVariable(expr.Keyword, expr);
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

        public object visitVariableExpr(Expr.Variable expr)
        {
            return lookUpVariable(expr.Name, expr);
        }

        public object lookUpVariable(Token name, Expr expr) {
            int distance = 0;
            if (locals.TryGetValue(expr, out distance)) {
                return env.getAt(distance, name.Lexeme);
            } else {
                return globals.get(name);
            }
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

        private void execute(Stmt statement) {
            statement.accept(this);
        }

        public void resolve(Expr expr, int depth) {
            locals[expr] = depth;
        }

        public void executeBlock(List<Stmt> statements, Envir envir)
        {
            Envir previous = env;
            try {
                env = envir;
                foreach (var s in statements) {
                    execute(s);
                }
            } finally {
                env = previous;
            }
        }


        public object visitBlockStmt(Stmt.Block stmt)
        {
            executeBlock(stmt.Statements, new Envir(env));
            return null;
        }

        public object visitClassStmt(Stmt.Class stmt) {
            env.define(stmt.Name.Lexeme, null);

            Dictionary<string, LoxFunction> methods = new Dictionary<string, LoxFunction>();
            foreach(Stmt.Function method in stmt.Methods) {
                LoxFunction function = new LoxFunction(method, env,
                    method.Name.Lexeme == "init");
                methods[method.Name.Lexeme] = function;
            }

            LoxClass klass = new LoxClass(stmt.Name.Lexeme, methods);
            env.assign(stmt.Name, klass);
            return null;
        }

        public object visitExpressionStmt(Stmt.Expression stmt)
        {
            object value = evaluate(stmt.Expr);
            return null;
        }

        public object visitFunctionStmt(Stmt.Function stmt)
        {
            LoxFunction function = new LoxFunction(stmt, env, false);
            env.define(stmt.Name.Lexeme, function);
            return null;
        }

        public object visitIfStmt(Stmt.If stmt)
        {
            if (isTruthy(evaluate(stmt.Cond))) {
                execute(stmt.ThenBranch);
            } else if (stmt.ElseBranch != null) {
                execute(stmt.ElseBranch);
            }
            return null;
        }

        public object visitPrintStmt(Stmt.Print stmt)
        {
            object value = evaluate(stmt.Expr);
            System.Console.WriteLine(stringify(value));
            return null;
        }


        public object visitReturnStmt(Stmt.Return stmt)
        {
            Object value = null;
            if (stmt.Value != null) {
                value = evaluate(stmt.Value);
            }
            throw new LoxReturn(value);
        }

        public object visitVarStmt(Stmt.Var stmt)
        {
            object value = null;
            if (stmt.Initilizer != null) {
                value = evaluate(stmt.Initilizer);
            }
            env.define(stmt.Name.Lexeme, value);
            return null;
        }

        public object visitWhileStmt(Stmt.While stmt)
        {
            while(isTruthy(evaluate(stmt.Cond))) {
                execute(stmt.Body);
            }
            return null;
        }

        public object visitAssignExpr(Expr.Assign expr)
        {
            object value = evaluate(expr.Value);
            env.assign(expr.Name, value);
            return value;
        }
    }
}
