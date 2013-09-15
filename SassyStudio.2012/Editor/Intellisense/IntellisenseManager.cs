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
        readonly ICssSchemaManager SchemaManager;
        readonly IEnumerable<ICompletionContextProvider> _ContextProviders;
        readonly IDictionary<SassCompletionContextType, IEnumerable<ICompletionValueProvider>> _ValueProviders;
        readonly ConcurrentDictionary<ISassDocument, IIntellisenseCache> Caches = new ConcurrentDictionary<ISassDocument, IIntellisenseCache>();

        [ImportingConstructor]
        public IntellisenseManager(ICssSchemaManager schemaManager, [ImportMany]IEnumerable<ICompletionContextProvider> contextProviders, [ImportMany]IEnumerable<ICompletionValueProvider> valueProviders)
        {
            SchemaManager = schemaManager;
            _ContextProviders = contextProviders.ToArray();
            _ValueProviders = valueProviders
                .SelectMany(x => x.SupportedContexts, (p, t) => new { Provider = p, Type = t })
                .GroupBy(x => x.Type)
                .ToDictionary(x => x.Key, x => (IEnumerable<ICompletionValueProvider>)x.Select(p => p.Provider).ToArray());
        }

        public IIntellisenseCache Get(ISassDocument document)
        {
            return Caches.GetOrAdd(document, key => CreateCache(key));
        }

        public IEnumerable<ICompletionContextProvider> ContextProviders { get { return _ContextProviders; } }

        public IEnumerable<ICompletionValueProvider> ValueProvidersFor(SassCompletionContextType type)
        {
            IEnumerable<ICompletionValueProvider> providers;
            if (_ValueProviders.TryGetValue(type, out providers))
                return providers;

            return Enumerable.Empty<ICompletionValueProvider>();
        }

        IIntellisenseCache CreateCache(ISassDocument document)
        {
            var cache = new IntellisenseCache(document, this);

            if (document.Stylesheet != null)
            {
                Task.Run(() => UpdateCache(cache, document));
            }
            else
            {
                // wait until we have a stylesheet to generate the cache
                document.StylesheetChanged += OnStylesheetChanged;
            }

            return cache;
        }

        private void OnStylesheetChanged(object sender, StylesheetChangedEventArgs e)
        {
            // unsubscribe from future notifications
            var document = sender as ISassDocument;
            document.StylesheetChanged -= OnStylesheetChanged;

            // update the cache
            IIntellisenseCache cache;
            if (Caches.TryGetValue(document, out cache))
                Task.Run(() => UpdateCache(cache, document));
        }

        private void UpdateCache(IIntellisenseCache cache, ISassDocument document)
        {
            try
            {
                var manager = new FileTextManager(document.Source);
                using (var scope = manager.Open())
                    cache.Update(document.Stylesheet, scope.Text);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Failed to update intellisense cache.");
            }
        }
    }
}
