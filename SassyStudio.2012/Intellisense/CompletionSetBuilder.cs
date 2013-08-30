using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;

namespace SassyStudio.Intellisense
{
    interface ICompletionSetBuilder
    {
        CompletionSet Build(SassCompletionContext context);
    }

    [Export(typeof(ICompletionSetBuilder))]
    class CompletionSetBuilder : ICompletionSetBuilder
    {
        readonly IEnumerable<ICompletionContextProvider> ContextProviders;
        readonly IDictionary<SassCompletionContextType, IEnumerable<ICompletionValueProvider>> ProviderRegistry;

        [ImportingConstructor]
        public CompletionSetBuilder([ImportMany] IEnumerable<ICompletionContextProvider> contextProviders, [ImportMany]IEnumerable<ICompletionValueProvider> valueProviders)
        {
            ContextProviders = contextProviders.ToArray();
            ProviderRegistry = (
                from provider in valueProviders
                from contextType in provider.SupportedContexts
                group provider by contextType into g
                select g
            ).ToDictionary(x => x.Key, x => (IEnumerable<ICompletionValueProvider>)x.ToArray());
        }

        public CompletionSet Build(SassCompletionContext context)
        {
            var contextTypes = new HashSet<SassCompletionContextType>(ContextProviders.SelectMany(x => x.GetContext(context)));
            if (contextTypes.Count == 0)
                return null;

            Logger.Log(string.Format("Current Type: {0}", context.Current.GetType().Name));
            return new CompletionSet("sass", "sass", context.TrackingSpan, Build(context, contextTypes), null);
        }

        private IEnumerable<Completion> Build(SassCompletionContext context, IEnumerable<SassCompletionContextType> types)
        {
            var observed = new HashSet<string>();
            foreach (var type in types)
            {
                IEnumerable<ICompletionValueProvider> providers;
                if (ProviderRegistry.TryGetValue(type, out providers))
                {
                    foreach (var value in providers.SelectMany(provider => provider.GetCompletions(type, context)))
                        if (observed.Add(value.DisplayText))
                            yield return value;
                }
            }
        }
    }
}
