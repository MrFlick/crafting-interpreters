using System;

namespace crafting_interpreters
{
    class Token {
        readonly TokenType Type;
        readonly String Lexeme;
        readonly Object Literal;
        readonly int Line;

        public Token(TokenType type, string lexeme, Object literal, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        public override string ToString() {
            return $"{Type} {Lexeme} {Literal}";
        }
    }
}
