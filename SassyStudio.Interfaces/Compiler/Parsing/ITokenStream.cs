using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public interface ITokenStream
    {
        int Length { get; }
        int Position { get; }
        Token Current { get; }
        Token Peek(int offset = 1);
        Token Advance(int offset = 1);
        Token Reverse(int offset = 1);
        Token SeekTo(int position);
    }
}
