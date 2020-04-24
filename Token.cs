using System;

namespace crafting_interpreters
{
    class Token {
        public readonly TokenType Type;
        public readonly String Lexeme;
        public readonly Object Literal;
        public readonly int Line;

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
