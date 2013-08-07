using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using SassyStudio.Compiler.Parsing;
using SassyStudio.Editor;

namespace SassyStudio.Intellisense
{
    class SassCompletionSource : ICompletionSource
    {
        private readonly List<ICompletionProvider> CompletionProviders;
        readonly ITextStructureNavigatorSelectorService NavigatorService;
        readonly ITextBuffer Buffer;
        readonly SassEditorDocument Document;
        public SassCompletionSource(ITextBuffer buffer, IEnumerable<ICompletionProvider> completionProviders, ITextStructureNavigatorSelectorService navigatorService)
        {
            CompletionProviders = completionProviders.ToList();
            CompletionProviders.TrimExcess();

            NavigatorService = navigatorService;
            Buffer = buffer;
            Document = SassEditorDocument.CreateFrom(buffer);
            Document.TreeChanged += OnTreeChanged;
            Tree = Document.Tree;
        }

        private ISassDocumentTree Tree { get; set; }

        private void OnTreeChanged(object sender, TreeChangedEventArgs e)
        {
            Tree = e.Tree;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            var point = session.GetTriggerPoint(Buffer);
            var span = FindTokenSpanAtPosition(point, session);
            completionSets.Add(new CompletionSet("sass_variables", "Sass Variables", span, null, CreateCompletions(span)));
        }

        private IEnumerable<Completion> CreateCompletions(ITrackingSpan span)
        {
            var tree = Tree;
            if (tree == null)
            {
                Logger.Log("No tree");
                return Enumerable.Empty<Completion>();
            }

            var position = span.GetStartPoint(Tree.SourceText).Position;

            Logger.Log(string.Format("Calculating completion from {0}", position));
            var path = GetTraversalPath(tree, position);
            var text = new SnapshotTextProvider(tree.SourceText);

            return CompletionProviders.SelectMany(provider => provider.GetCompletions(text, path, position));
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
            Document.TreeChanged -= OnTreeChanged;
        }

        static IReadOnlyCollection<ComplexItem> GetTraversalPath(ISassDocumentTree tree, int position)
        {
            var path = new List<ComplexItem>();
            var item = tree.Items.FindItemContainingPosition(position);
            if (item != null)
            {
                var current = (item as ComplexItem) ?? item.Parent;
                while (current != null)
                {
                    path.Add(current);
                    current = current.Parent;
                }
            }

            return path;
        }
    }
}
