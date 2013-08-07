using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Intellisense
{
    public interface ICompletionProvider
    {
        IEnumerable<Completion> GetCompletions(ITextProvider text, IEnumerable<ComplexItem> containers, int end);
    }
}
