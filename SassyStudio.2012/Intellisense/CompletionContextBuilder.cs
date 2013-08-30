using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using SassyStudio.Compiler.Parsing;
using SassyStudio.Editor;

namespace SassyStudio.Intellisense
{
    interface ICompletionContextBuilder
    {
        SassCompletionContext Create(ITrackingSpan span, SassEditorDocument document);
    }

    [Export(typeof(ICompletionContextBuilder))]
    class CompletionContextBuilder : ICompletionContextBuilder
    {
        public SassCompletionContext Create(ITrackingSpan span, SassEditorDocument document)
        {
            var tree = document.Tree;
            var text = new SnapshotTextProvider(tree.SourceText);
            var position = span.GetStartPoint(tree.SourceText).Position;

            var current = Find(tree, position);
            var path = CreateTraversalPath(current);

            return new SassCompletionContext(tree.SourceText, span, current, path);
        }

        private ParseItem Find(ISassDocumentTree tree, int position)
        {
            if (tree == null)
                return null;

            var item = tree.Items.FindItemContainingPosition(position);
            while (item != null)
            {
                // if we have a complex item, stop searching
                if (item is ComplexItem)
                    break;

                item = item.Parent;
            }

            return item;
        }

        static IEnumerable<ComplexItem> CreateTraversalPath(ParseItem item)
        {
            var path = new LinkedList<ComplexItem>();
            if (item == null)
                return path;

            var current = item;
            while (current != null)
            {
                // only consider complex items in path
                if (current is ComplexItem)
                    path.AddLast(current as ComplexItem);

                current = current.Parent;
            }

            return path;
        }
    }
}
