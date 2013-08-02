using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Lexing
{
    /// <summary>
    /// Translates a character stream into a normalized view
    /// </summary>
    public interface ITextStream
    {
        int Position { get; }
        int Length { get; }
        char Current { get; }
        bool Advance(int offset = 1);
        bool Reverse(int offset = 1);
        void SeekTo(int index);
        char Peek(int offset);
    }
}
