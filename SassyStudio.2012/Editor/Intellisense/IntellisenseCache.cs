using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    class IntellisenseCache : IIntellisenseCache
    {
        readonly ISassDocument Document;

        public IntellisenseCache(ISassDocument document, IEnumerable<ICompletionContextProvider> contextProviders, IDictionary<SassCompletionContextType, IEnumerable<ICompletionValueProvider>> valueProviders)
        {
            Document = document;
        }

        public void Update(ISassStylesheet stylesheet, ITextProvider text)
        {

        }
    }
}
