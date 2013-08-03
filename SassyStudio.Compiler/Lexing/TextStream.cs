using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;

namespace SassyStudio.Compiler.Lexing
{
    class TextStream : ITextStream
    {
        private readonly NormalizingTextStream Inner;
        public TextStream(ITextProvider text)
        {
            Inner = new NormalizingTextStream(text);
        }

        public int Position { get { return Inner.Position; } }
        public int Length { get { return Inner.Length; } }
        public char Current { get { return Inner.Current; } }

        public bool Advance(int offset = 1)
        {
            while (offset-- > 0)
                Inner.Advance();

            return Position < Length;
        }

        public bool Reverse(int offset = 1)
        {
            while (offset-- > 0)
                Inner.Reverse();

            return Position > 0;
        }

        public void SeekTo(int index)
        {
            if (Position > index)
            {
                Reverse(Position - index);
            }
            else if (Position < index)
            {
                Advance(index - Position);
            }
        }

        public char Peek(int offset)
        {
            var position = Position;
            SeekTo(Position + offset);
            
            var peeked = Current;
            SeekTo(position);

            return peeked;
        }
    }
}
