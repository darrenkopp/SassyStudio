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
        readonly IEnumerable<ISassCompletionAugmenter> CompletionAugmenters;
        readonly ITextStructureNavigatorSelectorService NavigatorService;
        readonly ITextBuffer Buffer;
        readonly SassEditorDocument Document;
        public SassCompletionSource(ITextBuffer buffer, IEnumerable<ISassCompletionAugmenter> completionAugmenters, ITextStructureNavigatorSelectorService navigatorService)
        {
            var augmenters = completionAugmenters.ToList();
            augmenters.TrimExcess();
            CompletionAugmenters = augmenters;

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
            var tree = Tree;
            if (tree != null)
            {
                var span = FindTokenSpanAtPosition(session);
                var context = CreateContext(span, tree);

                var allCompletions = new LinkedList<Completion>();
                IEnumerable<Completion> allBuilders = null;
                foreach (var augmenter in CompletionAugmenters)
                {
                    var builder = augmenter.GetBuilder(context);
                    if (builder != null)
                        allBuilders = (allBuilders ?? Enumerable.Empty<Completion>()).Concat(builder);
                }

                if (allCompletions != null || allBuilders != null)
                    completionSets.Add(new CompletionSet("sass", "sass", context.TrackingSpan, null, allBuilders));
            }
        }

        private SassCompletionContext CreateContext(ITrackingSpan span, ISassDocumentTree tree)
        {
            var text = new SnapshotTextProvider(tree.SourceText);
            var current = tree.Items.FindItemContainingPosition(span.GetStartPoint(tree.SourceText).Position);
            var path = CreateTraversalPath(current);

            return new SassCompletionContext(tree.SourceText, span, current, path);
        }

        private IEnumerable<ComplexItem> CreateTraversalPath(ParseItem item)
        {
            var path = new LinkedList<ComplexItem>();
            if (item == null)
                return path;

            var current = (item as ComplexItem) ?? item.Parent;
            while (current != null)
            {
                path.AddLast(current);
                current = current.Parent;
            }

            return path;
        }

        private ITrackingSpan FindTokenSpanAtPosition(ICompletionSession session)
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
    }
}
