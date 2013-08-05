using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;

namespace SassyStudio.Intellisense
{
    class SassCompletionSource : ICompletionSource
    {
        readonly ITextStructureNavigatorSelectorService NavigatorService;
        readonly ITextBuffer Buffer;
        public SassCompletionSource(ITextBuffer buffer,ITextStructureNavigatorSelectorService navigatorService)
        {
            NavigatorService = navigatorService;
            Buffer = buffer;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
        }

        private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint trackingPoint, ICompletionSession session)
        {
            SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
            var navigator = NavigatorService.GetTextStructureNavigator(Buffer);
            TextExtent extent = navigator.GetExtentOfWord(currentPoint);
            return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
        }

        public void Dispose()
        {
        }
    }
}
