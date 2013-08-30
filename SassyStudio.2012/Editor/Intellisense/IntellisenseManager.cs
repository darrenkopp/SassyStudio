using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(IIntellisenseManager))]
    class IntellisenseManager : IIntellisenseManager
    {
        readonly IEnumerable<ICompletionContextProvider> ContextProviders;
        readonly IDictionary<SassCompletionContextType, IEnumerable<ICompletionValueProvider>> ValueProviders;
        readonly ConcurrentDictionary<ISassDocument, IIntellisenseCache> Caches = new ConcurrentDictionary<ISassDocument, IIntellisenseCache>();
        readonly CancellationToken ShutdownToken;

        [ImportingConstructor]
        public IntellisenseManager([ImportMany]IEnumerable<ICompletionContextProvider> contextProviders, [ImportMany]IEnumerable<ICompletionValueProvider> valueProviders)
        {
            ContextProviders = contextProviders;
            ValueProviders = valueProviders
                .SelectMany(x => x.SupportedContexts, (p, t) => new { Provider = p, Type = t })
                .GroupBy(x => x.Type)
                .ToDictionary(x => x.Key, x => (IEnumerable<ICompletionValueProvider>)x.Select(p => p.Provider).ToArray());

            ShutdownToken = SassyStudioPackage.Instance.ShutdownToken;
        }

        public IEnumerable<SassCompletionContextType> GetCompletionContextTypes(ICompletionContext context)
        {
            return ContextProviders.SelectMany(x => x.GetContext(context));
        }

        public IEnumerable<ICompletionValueProvider> GetCompletions(SassCompletionContextType contextType)
        {
            IEnumerable<ICompletionValueProvider> providers;
            if (ValueProviders.TryGetValue(contextType, out providers))
                return providers;

            return Enumerable.Empty<ICompletionValueProvider>();
        }

        public IIntellisenseCache Get(ISassDocument document)
        {
            return Caches.GetOrAdd(document, key => CreateCache(key));
        }

        IIntellisenseCache CreateCache(ISassDocument document)
        {
            var cache = new IntellisenseCache(document, ContextProviders, ValueProviders);
            document.StylesheetChanged += OnStylesheetChanged;

            // start initial cache update
            if (document.Stylesheet != null)
                UpdateCache(cache, document);

            return cache;
        }

        private void OnStylesheetChanged(object sender, StylesheetChangedEventArgs e)
        {
            var document = sender as ISassDocument;
            IIntellisenseCache cache;
            if (Caches.TryGetValue(document, out cache))
                UpdateCache(cache, document);
        }

        private void UpdateCache(IIntellisenseCache cache, ISassDocument document)
        {
            Task.Run(() => cache.Update(document.Stylesheet), ShutdownToken);
        }
    }
}
