using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(IIntellisenseManager))]
    class IntellisenseManager : IIntellisenseManager
    {
        readonly ConcurrentDictionary<ISassDocument, IIntellisenseCache> Caches = new ConcurrentDictionary<ISassDocument, IIntellisenseCache>();
        
        [ImportingConstructor]
        public IntellisenseManager()
        {
            
        }

        public IIntellisenseCache Get(ISassDocument document)
        {
            return Caches.GetOrAdd(document, key => CreateCache(key));
        }

        IIntellisenseCache CreateCache(ISassDocument document)
        {
            // TODO: subscribe to document changes
            // TODO: pass along context providers
            // TODO: 
            throw new NotImplementedException();
        }
    }
}
