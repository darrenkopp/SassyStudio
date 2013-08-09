using System.Collections.Generic;
using Microsoft.VisualStudio.Language.Intellisense;

namespace SassyStudio.Intellisense
{
    interface ISassCompletionProvider
    {
        IEnumerable<Completion> GetCompletions(SassCompletionContext context);
    }
}
