using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using SassyStudio.Compiler.Parsing;
using SassyStudio.Editor;

namespace SassyStudio.Intellisense
{
    class SassCompletionSource : ICompletionSource
    {
        readonly ICompletionManager Manager;
        readonly ITextStructureNavigatorSelectorService NavigatorService;

        public SassCompletionSource(ICompletionManager manager, ITextStructureNavigatorSelectorService navigatorService)
        {
            Manager = manager;
            NavigatorService = navigatorService;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            var span = FindTokenSpanAtPosition(session);
            var set = Manager.CreateCompletionSetFor(span, session);
            if (set != null)
                completionSets.Add(set);
        }

        private ITrackingSpan FindTokenSpanAtPosition(ICompletionSession session)
        {
            var navigator = NavigatorService.GetTextStructureNavigator(session.TextView.TextBuffer);
            var position = (session.TextView.Caret.Position.BufferPosition)-1;
            var extent = navigator.GetExtentOfWord(position);

            return position.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
        }

        public void Dispose()
        {
            Manager.Dispose();
        }
    }
}
