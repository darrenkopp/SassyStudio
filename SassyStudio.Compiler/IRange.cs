using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler
{
    public interface IRange
    {
        int Start { get; }
        int End { get; }
        int Length { get; }
    }
}
