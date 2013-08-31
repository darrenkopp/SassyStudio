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
        readonly IIntellisenseManager IntellisenseManager;

        public IntellisenseCache(ISassDocument document, IIntellisenseManager intellisenseManager)
        {
            Document = document;
            IntellisenseManager = intellisenseManager;
        }

        public void Update(ISassStylesheet stylesheet, ITextProvider text)
        {
            
        }
    }
}
