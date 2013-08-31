using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio
{
    public interface IRange
    {
        int Start { get; }
        int End { get; }
        int Length { get; }
    }
}
