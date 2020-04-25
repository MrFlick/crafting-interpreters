using System.Collections.Generic;

namespace crafting_interpreters
{
    class Parser {
        private readonly List<Token> Tokens;
        private int current = 0;

        private class ParseError : System.Exception {
            public ParseError() : base() { }
            public ParseError(string message) : base(message) { }
        }

        public Parser(List<Token> tokens) {
            Tokens = tokens;
        }

        public List<Stmt> parse() {
            List<Stmt> statements = new List<Stmt>();
            while(!isAtEnd()) {
                statements.Add(declaration());
            }
            return statements;
        }

        private Expr expression() {
            return assignment();
        }

        private Expr assignment() {
            Expr expr = ternary();
            if (match(TokenType.EQUAL)) {
                Token equals = previous();
                Expr value = assignment();
                if (expr is Expr.Variable) {
                    Token name = ((Expr.Variable)expr).Name;
                    return new Expr.Assign(name, value);
                }
                error(equals, "Invalid assignment target.");
            }
            return expr;
        }

        private Stmt declaration() {
            try {
                if (match(TokenType.VAR)) return varDeclaration();
                return statement();
            } catch (ParseError) {
                synchronize();
                return null;
            }
        }

        private Stmt statement() {
            if (match(TokenType.PRINT)) {
                return printStatement();
            }
            if (match(TokenType.LEFT_BRACE)) {
                return new Stmt.Block(block());
            }
            return expressionStatement();
        }

        private Stmt printStatement() {
            Expr value = expression();
            consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
        }

        private Stmt varDeclaration() {
            Token name = consume(TokenType.IDENTIFIER, "Expect variable name");

            Expr initializer = null;
            if (match(TokenType.EQUAL)) {
                initializer = expression();
            }

            consume(TokenType.SEMICOLON, "Expect ';' after variable decl.");
            return new Stmt.Var(name, initializer);
        }

        private Stmt expressionStatement() {
            Expr expr = expression();
            consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }

        private List<Stmt> block() {
            List<Stmt> statements = new List<Stmt>();

            while(!check(TokenType.RIGHT_BRACE) && !isAtEnd()) {
                statements.Add(declaration());
            }

            consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        private Expr ternary() {
            // expression     → equality ( "?" equality ":" equality )?;
            Expr expr = equality();
            if (match(TokenType.QUEST)) {
                Expr ifTrue = expression();
                consume(TokenType.COLON, "Expected : after ? expresion");
                Expr ifFalse = expression();
                expr = new Expr.Ternary(expr, ifTrue, ifFalse);
            }
            return expr;
        }

        private Expr equality() {
            // equality       → comparison ( ( "!=" | "==" ) comparison )* ;
            Expr expr = comparison();
            while(match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) {
                Token op = previous();
                Expr right = comparison();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        private Expr comparison() {
            // comparison     → addition ( ( ">" | ">=" | "<" | "<=" ) addition )* ;
            Expr expr = addition();
            while(match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)) {
                Token op = previous();
                Expr right = addition();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        private Expr addition() {
            // addition       → multiplication ( ( "-" | "+" ) multiplication )* ;
            Expr expr = multiplication();
            while(match(TokenType.MINUS, TokenType.PLUS)) {
                Token op = previous();
                Expr right = multiplication();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        private Expr multiplication() {
            // multiplication → unary ( ( "/" | "*" ) unary )* ;
            Expr expr = unary();
            while(match(TokenType.SLASH, TokenType.STAR)) {
                Token op = previous();
                Expr right = unary();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        private Expr unary() {
            // unary          → ( "!" | "-" ) unary
            //                | primary ;
            if(match(TokenType.BANG, TokenType.MINUS)) {
                Token op = previous();
                Expr right = primary();
                return new Expr.Unary(op, right);
            } else {
                return primary();
            }
        }

        private Expr primary() {
            // primary        → NUMBER | STRING | "false" | "true" | "nil"
            //                | "(" expression ")" ;
            if (match(TokenType.FALSE)) return new Expr.Literal(false);
            if (match(TokenType.TRUE)) return new Expr.Literal(true);
            if (match(TokenType.NIL)) return new Expr.Literal(null);

            if (match(TokenType.NUMBER, TokenType.STRING)) {
                return new Expr.Literal(previous().Literal);
            }
            if(match(TokenType.IDENTIFIER)) {
                return new Expr.Variable(previous());
            }
            if (match(TokenType.LEFT_PAREN)) {
                Expr expr = expression();
                consume(TokenType.RIGHT_PAREN, "Expected ) after expresion");
                return new Expr.Grouping(expr);                
            }

            throw error(peek(), "Expect expression.");
        }

        private bool match(params TokenType[] tokenTypes) {
            foreach(TokenType tt in tokenTypes) {
                if (check(tt)) {
                    advance();
                    return true;
                }
            }
            return false;
        }

        private Token consume(TokenType tokenType, string message) {
            if (check(tokenType)) return advance();
            throw error(peek(), message);
        }

        private bool check(TokenType tokenType) {
            if (isAtEnd()) return false;
            return peek().Type == tokenType;
        }

        private Token advance() {
            if (!isAtEnd()) {current++;}
            return previous();

        }

        private bool isAtEnd() {
            return peek().Type == TokenType.EOF;
        }

        private Token peek() {
            return Tokens[current];
        }

        private Token previous() {
            return Tokens[current -1];
        }

        private ParseError error(Token token, string message) {
            Lox.error(token, message);
            synchronize();
            return new ParseError();
        }

        private void synchronize() {
            advance();
            while(!isAtEnd()) {
                if (previous().Type == TokenType.SEMICOLON) return;
                switch(peek().Type) {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }
                advance();
            }
        }
    }
}
