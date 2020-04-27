using System.Collections.Generic;
using System.Linq;

namespace crafting_interpreters
{
    class Resolver : Expr.Visitor<object>, Stmt.Visitor<object> {
        private readonly Interpreter Interpreter;
        private readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
        private FunctionType currentFunction = FunctionType.NONE;
        private ClassType currentClass = ClassType.NONE;

        public Resolver(Interpreter interpreter) {
            Interpreter = interpreter;
        }

        private enum FunctionType {
            NONE,
            FUNCTION,
            INITIALIZER,
            METHOD,
        }
        private enum ClassType {
            NONE,
            CLASS,
        }

        public void resolve(List<Stmt> statements) {
            foreach (Stmt statement in statements) {
                resolve(statement);
            }
        }

        private void resolve(Stmt stmt) {
            stmt.accept(this);
        }

        private void resolve(Expr expr) {
            expr.accept(this);
        }

        private void beginScope() {
            scopes.Push(new Dictionary<string, bool>());
        }

        private void endScope() {
            scopes.Pop();
        }

        private void declare(Token name) {
            if(scopes.Count==0) return;

            var scope = scopes.Peek();
            if(scope.ContainsKey(name.Lexeme)) {
                Lox.error(name, "Variable with this name already delared in this scope");
            }
            scope[name.Lexeme] = false;
        }

        private void define(Token name) {
            if (scopes.Count==0) return;
            scopes.Peek()[name.Lexeme] = true;
        }

        private void resolveLocal(Expr expr, Token name) {
            for (int i= 0; i<scopes.Count; i++) {
                if (scopes.ElementAt(i).ContainsKey(name.Lexeme)) {
                    Interpreter.resolve(expr, i);
                    return;
                }
            }
        }

        private void resolveFunction(Stmt.Function function, FunctionType type) {
            FunctionType enclosingFunction = currentFunction;
            currentFunction = type;
            beginScope();
            foreach(Token param in function.Parameters) {
                declare(param);
                define(param);
            }
            resolve(function.Body);
            endScope();
            currentFunction = enclosingFunction;
        }

        public object visitAssignExpr(Expr.Assign expr)
        {
            resolve(expr.Value);
            resolveLocal(expr, expr.Name);
            return null;
        }

        public object visitBinaryExpr(Expr.Binary expr)
        {
            resolve(expr.Left);
            resolve(expr.Right);
            return null;
        }

        public object visitBlockStmt(Stmt.Block stmt)
        {
            beginScope();
            resolve(stmt.Statements);
            endScope();
            return null;
        }

        public object visitClassStmt(Stmt.Class stmt)
        {
            ClassType enclosingClass = currentClass;
            currentClass = ClassType.CLASS;

            declare(stmt.Name);
            define(stmt.Name);

            beginScope();
            scopes.Peek()["this"] = true;
            
            foreach(Stmt.Function method in stmt.Methods) {
                FunctionType declaration = FunctionType.METHOD;
                if (method.Name.Lexeme == "init") {
                    declaration = FunctionType.INITIALIZER;
                }
                resolveFunction(method, declaration);
            }
            
            endScope();

            currentClass = enclosingClass;
            return null;
        }

        public object visitCallExpr(Expr.Call expr)
        {
            resolve(expr.Callee);
            foreach(Expr arg in expr.Arguments) {
                resolve(arg);
            }
            return null;
        }

        public object visitGetExpr(Expr.Get expr) {
            resolve(expr.Instance);
            return null;
        }

        public object visitExpressionStmt(Stmt.Expression stmt)
        {
            resolve(stmt.Expr);
            return null;
        }

        public object visitFunctionStmt(Stmt.Function stmt)
        {
            declare(stmt.Name);
            define(stmt.Name);

            resolveFunction(stmt, FunctionType.FUNCTION);
            return null;
        }

        public object visitGroupingExpr(Expr.Grouping expr)
        {
            resolve(expr.Expression);
            return null;
        }

        public object visitIfStmt(Stmt.If stmt)
        {
            resolve(stmt.Cond);
            resolve(stmt.ThenBranch);
            if(stmt.ElseBranch != null) {
                resolve(stmt.ElseBranch);
            }
            return null;
        }

        public object visitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }

        public object visitLogicalExpr(Expr.Logical expr)
        {
            resolve(expr.Left);
            resolve(expr.Right);
            return null;
        }

        public object visitSetExpr(Expr.Set expr) {
            resolve(expr.Value);
            resolve(expr.Instance);
            return null;
        }

        public object visitThisExpr(Expr.This expr) {
            if (currentClass == ClassType.NONE) {
                Lox.error(expr.Keyword,
                    "Cannot use `this` outside of class");
                return null;
            }
            resolveLocal(expr, expr.Keyword);
            return null;
        }

        public object visitPrintStmt(Stmt.Print stmt)
        {
            if(stmt.Expr != null) {
                resolve(stmt.Expr);
            }
            return null;
        }

        public object visitReturnStmt(Stmt.Return stmt)
        {
            if(currentFunction == FunctionType.NONE) {
                Lox.error(stmt.Keyword, "Cannot return from top-level code");
            }
            if(stmt.Value != null) {
                if (currentFunction == FunctionType.INITIALIZER) {
                    Lox.error(stmt.Keyword,
                        "Cannot return a value from initilizer;");
                }
                resolve(stmt.Value);
            }
            return null;
        }

        public object visitTernaryExpr(Expr.Ternary expr)
        {
            resolve(expr.Cond);
            resolve(expr.IfTrue);
            resolve(expr.IfFalse);
            return null;
        }

        public object visitUnaryExpr(Expr.Unary expr)
        {
            resolve(expr.Right);
            return null;
        }

        public object visitVariableExpr(Expr.Variable expr)
        {
            bool initialized;
            if(scopes.Count>0 && scopes.Peek().TryGetValue(expr.Name.Lexeme, out initialized) && initialized==false) {
                Lox.error(expr.Name, "Cannot read local variable in it's own initializer");
            }
            resolveLocal(expr, expr.Name);
            return null;
        }

        public object visitVarStmt(Stmt.Var stmt)
        {
            declare(stmt.Name);
            if(stmt.Initilizer != null) {
                resolve(stmt.Initilizer);
            }
            define(stmt.Name);
            return null;
        }

        public object visitWhileStmt(Stmt.While stmt)
        {
            resolve(stmt.Cond);
            resolve(stmt.Body);
            return null;
        }
    }
}
