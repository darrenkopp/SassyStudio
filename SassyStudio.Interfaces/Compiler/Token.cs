using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler
{
    public struct Token
    {
        public readonly TokenType Type;
        public readonly int Start;
        public readonly int Length;

        public Token(TokenType type, int start, int length = 1)
        {
            Type = type;
            Start = start;
            Length = length;
        }

        public int End { get { return Start + Length; } }

        public static Token CreateEmpty(TokenType type, int position)
        {
            return new Token(type, position, 0);
        }

        public static Token Create(TokenType type, int position, int length = 1)
        {
            return new Token(type, position, length);
        }
    }
}
