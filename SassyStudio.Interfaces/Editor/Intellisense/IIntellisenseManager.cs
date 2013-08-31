using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    public interface IIntellisenseManager
    {
        IIntellisenseCache Get(ISassDocument document);
        IEnumerable<ICompletionContextProvider> ContextProviders { get; }
        IEnumerable<ICompletionValueProvider> ValueProvidersFor(SassCompletionContextType type);
    }
}
