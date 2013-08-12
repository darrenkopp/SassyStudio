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

            var current = tree.Items.FindItemContainingPosition(position);
            if (current != null && current is TokenItem)
                current = current.Parent;
            var path = CreateTraversalPath(current);

            return new SassCompletionContext(tree.SourceText, span, current, path);
        }

        static IEnumerable<ComplexItem> CreateTraversalPath(ParseItem item)
        {
            var path = new LinkedList<ComplexItem>();
            if (item == null)
                return path;

            Logger.Log(string.Format("Current Type: {0}", item.GetType().Name));
            var current = item.Parent;
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
