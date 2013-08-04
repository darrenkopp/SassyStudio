using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler
{
    public interface ITextProvider
    {
        int Length { get; }
        char this[int index] { get; }
        string GetText(int start, int length);
        bool CompareOrdinal(int start, string value);
        bool StartsWithOrdinal(int start, string value);
    }
}
