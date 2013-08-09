using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;

namespace SassyStudio.Intellisense
{
    interface ICompletionValueProvider
    {
        IEnumerable<SassCompletionContextType> SupportedContexts { get; }

        IEnumerable<Completion> GetCompletions(SassCompletionContextType type, SassCompletionContext context);
    }
}
