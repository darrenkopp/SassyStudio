using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler
{
    public interface IParsingRequest
    {
        ITextProvider Text { get; }
        ISassDocument Document { get; }
        DateTime RequestedOn { get; }
        bool IsCancelled { get; }
    }
}
