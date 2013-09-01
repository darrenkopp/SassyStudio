using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;

namespace SassyStudio.Compiler.Lexing
{
    class NormalizingTextStream
    {
        private readonly ITextProvider Text;
        public NormalizingTextStream(ITextProvider text)
        {
            Text = text;
            UpdateCharacter();
        }

        public int Position { get; private set; }
        public int Length { get { return Text.Length; } }
        public char Current { get; private set; }

        public void Advance()
        {
            Position++;

            //if (Position >= 0 && Position < (Text.Length - 1))
            //{
            //    // if we are on \r and next is \n, skip the \r and jump to \n
            //    if (Text[Position] == '\r' && Text[Position + 1] == '\n')
            //        Position++;
            //}

            UpdateCharacter();
        }

        public void Reverse()
        {
            Position--;

            //if (Position >= 0 && Position < (Text.Length - 1))
            //{
            //    // if we are on \r and next is \n, skip it
            //    if (Text[Position] == '\r' && Text[Position + 1] == '\n')
            //        Position--;
            //}

            UpdateCharacter();
        }

        private void UpdateCharacter()
        {
            if (Position < 0 || Position >= Text.Length)
            {
                Current = '\0';
            }
            else
            {
                Current = Text[Position];
            }
        }
    }
}
