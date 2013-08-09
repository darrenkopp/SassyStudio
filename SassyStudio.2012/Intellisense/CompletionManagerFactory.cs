using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace SassyStudio.Intellisense
{
    interface ICompletionManagerFactory
    {
        ICompletionManager CreateFrom(ITextBuffer buffer);
    }

    [Export(typeof(ICompletionManagerFactory))]
    class CompletionManagerFactory : ICompletionManagerFactory
    {
        [Import]
        ICompletionBroker Broker { get; set; }

        [Import]
        ICompletionSetBuilder CompletionBuilder { get; set; }

        [Import]
        ICompletionContextBuilder ContextBuilder { get; set; }

        public ICompletionManager CreateFrom(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new CompletionManager(buffer, ContextBuilder, CompletionBuilder, Broker));
        }
    }
}
