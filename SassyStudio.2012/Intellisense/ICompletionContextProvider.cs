using System.Collections.Generic;

namespace SassyStudio.Intellisense
{
    interface ICompletionContextProvider
    {
        IEnumerable<SassCompletionContextType> GetContext(SassCompletionContext context);
    }
}
