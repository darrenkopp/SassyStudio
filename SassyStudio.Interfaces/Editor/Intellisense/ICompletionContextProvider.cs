using System.Collections.Generic;

namespace SassyStudio.Editor.Intellisense
{
    public interface ICompletionContextProvider
    {
        IEnumerable<SassCompletionContextType> GetContext(ICompletionContext context);
    }
}
