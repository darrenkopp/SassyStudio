using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using SassyStudio.Editor;

namespace SassyStudio.Intellisense
{
    class CompletionManager : ICompletionManager
    {
        readonly ConcurrentDictionary<ICompletionSession, CompletionSet> TrackedSessions = new ConcurrentDictionary<ICompletionSession, CompletionSet>();
        readonly ITextBuffer Buffer;
        readonly ICompletionContextBuilder ContextBuilder;
        readonly ICompletionSetBuilder CompletionBuilder;
        readonly ICompletionBroker Broker;
        readonly SassEditorDocument Document;

        public CompletionManager(ITextBuffer buffer, ICompletionContextBuilder contextBuilder, ICompletionSetBuilder completionBuilder, ICompletionBroker broker)
        {
            Buffer = buffer;
            ContextBuilder = contextBuilder;
            CompletionBuilder = completionBuilder;
            Broker = broker;

            Document = SassEditorDocument.CreateFrom(buffer);
            Document.TreeChanged += OnTreeChanged;
        }

        public CompletionSet CreateCompletionSetFor(ITrackingSpan span, ICompletionSession session)
        {
            if (disposed)
                return null;

            return TrackedSessions.GetOrAdd(session, s => TrackSessionAndGenerateSet(s, ContextBuilder.Create(span, Document)));
        }

        private CompletionSet TrackSessionAndGenerateSet(ICompletionSession session, SassCompletionContext context)
        {
            var set = CompletionBuilder.Build(context);
            session.Dismissed += OnSessionDismissed;

            return set;
        }

        private void OnTreeChanged(object sender, TreeChangedEventArgs e)
        {
            var sessions = TrackedSessions.ToList().Select(x => x.Key);
            foreach (var session in sessions.Where(x => !x.IsDismissed))
                session.Recalculate();
        }

        private void OnSessionDismissed(object sender, System.EventArgs e)
        {
            var source = sender as ICompletionSession;
            source.Dismissed -= OnSessionDismissed;

            CompletionSet _;
            TrackedSessions.TryRemove(source, out _);
        }

        bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;

                Document.TreeChanged -= OnTreeChanged;

                foreach (var session in TrackedSessions.ToList().Where(x => !x.Key.IsDismissed))
                    session.Key.Dismiss();

                TrackedSessions.Clear();
            }
        }
    }
}
