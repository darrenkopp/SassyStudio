using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    public interface ICompletionContext
    {
        ISassDocument Document { get; }
        IIntellisenseCache Cache { get; }
        ITextProvider DocumentTextProvider { get; }
        ParseItem Current { get; }
        //ParseItem Predecessor { get; }
        int Position { get; }
    }
}
