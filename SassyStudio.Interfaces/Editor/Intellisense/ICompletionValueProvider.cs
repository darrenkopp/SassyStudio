using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Editor.Intellisense
{
    public interface ICompletionValueProvider
    {
        IEnumerable<SassCompletionContextType> SupportedContexts { get; }

        IEnumerable<ICompletionValue> GetCompletions(SassCompletionContextType type, ICompletionContext context);
    }
}
