using System;
using System.Collections.Generic;

namespace crafting_interpreters
{
    class Scanner {
        private readonly String Source;
        private readonly List<Token> Tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;

        static Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType> {
            {"and", TokenType.AND},
            {"class", TokenType.CLASS},
            {"else", TokenType.ELSE},
            {"false", TokenType.FALSE},
            {"for", TokenType.FOR},
            {"fun", TokenType.FUN},
            {"if", TokenType.IF},
            {"nil", TokenType.NIL},
            {"or", TokenType.OR},
            {"print", TokenType.PRINT},
            {"return", TokenType.RETURN},
            {"super", TokenType.SUPER},
            {"this", TokenType.THIS},
            {"true", TokenType.TRUE},
            {"var", TokenType.VAR},
            {"while", TokenType.WHILE},
        };

        public Scanner(string source)
        {
            Source = source;
        }

        public List<Token> scanTokens() {
            while(!isAtEnd()) {
                start = current;
                scanToken();
            }

            Tokens.Add(new Token(TokenType.EOF, "", null, line));
            return Tokens;
        }

        private void scanToken() {
            char c = advance();
            switch(c) {
                case '(': addToken(TokenType.LEFT_PAREN); break;
                case ')': addToken(TokenType.RIGHT_PAREN); break;
                case '{': addToken(TokenType.LEFT_BRACE); break;
                case '}': addToken(TokenType.RIGHT_BRACE); break;
                case ',': addToken(TokenType.COMMA); break;
                case '.': addToken(TokenType.DOT); break;
                case '-': addToken(TokenType.MINUS); break;
                case '+': addToken(TokenType.PLUS); break;
                case ';': addToken(TokenType.SEMICOLON); break;
                case '*': addToken(TokenType.STAR); break;
                case '?': addToken(TokenType.QUEST); break;
                case ':': addToken(TokenType.COLON); break;
                case '!': addToken(match('=')? TokenType.BANG_EQUAL: TokenType.BANG); break;
                case '=': addToken(match('=')? TokenType.EQUAL_EQUAL: TokenType.EQUAL); break;
                case '<': addToken(match('=')? TokenType.LESS_EQUAL: TokenType.LESS); break;
                case '>': addToken(match('=')? TokenType.GREATER_EQUAL: TokenType.GREATER); break;
                case '/':
                    if (match('/')) {
                        while(peek() != '\n' && !isAtEnd()) advance();
                    } else {
                        addToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;
                case '"': scanString(); break;
                default:
                    if (isDigit(c)) {
                        scanNumber();
                    } else if (isAlpha(c)) {
                        scanIdentifier();

                    } else {
                        Lox.error(line, $"Unexpected character '{c}'");
                    };
                    break;
            }
        }

        private void scanIdentifier() {
            while(isAlphaNumeric(peek())) advance();

            string text = Source.Substring(start, current-start);
            TokenType type = Keywords.GetValueOrDefault(text, TokenType.IDENTIFIER);
            addToken(type);
        }

        private void scanString() {
            while(peek() != '"' && !isAtEnd()) {
                if (peek() == '\n') line++;
                advance();
            }
            if (isAtEnd()) {
                Lox.error(line, "Unterminated string");
                return;
            }
            advance();

            string value = Source.Substring(start + 1, current-start-2);
            addToken(TokenType.STRING, value);
        }

        private void scanNumber() {
            while(isDigit(peek())) advance();

            if (peek() == '.' && isDigit(peekNext())) {
                advance();
            }
            while (isDigit(peek())) advance();

            addToken(TokenType.NUMBER,
            Double.Parse(Source.Substring(start, current-start)));
        }

        private bool match(char expected) {
            if (isAtEnd()) return false;
            if(Source[current] != expected) return false;
            current ++;
            return true;
        }

        private char peek() {
            if (isAtEnd()) return '\0';
            return Source[current];
        }
        private char peekNext() {
            if (current + 1 >= Source.Length) return '\0';
            return Source[current+1];
        }

        private bool isAlpha(char c) {
            return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '_';
        }

        private bool isAlphaNumeric(char c) {
            return isAlpha(c) || isDigit(c);
        }

        private bool isDigit(char c) {
            return c>= '0' && c <= '9';
        }

        private char advance() {
            current++;
            return Source[current-1];
        }

        private void addToken(TokenType type) {
            addToken(type, null);
        }

        private void addToken(TokenType type, Object literal) {
            string text = Source.Substring(start, current-start);
            Tokens.Add(new Token(type, text, literal, line));
        }

        private bool isAtEnd() {
            return current >= Source.Length;
        }
    }
}
