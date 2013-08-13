using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using SassyStudio.Compiler;
using SassyStudio.Compiler.Parsing;
using SassyStudio.Editor;

namespace SassyStudio.Classification
{
    class SassClassifier : IClassifier
    {
        readonly IClassificationTypeRegistryService Registry;
        readonly SassEditorDocument Editor;

        public SassClassifier(ITextBuffer buffer, IClassificationTypeRegistryService registry)
        {
            Registry = registry;
            Editor = SassEditorDocument.CreateFrom(buffer);
            Editor.TreeChanged += OnTreeChanged;
        }

        private ISassDocumentTree Tree { get; set; }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var results = new List<ClassificationSpan>();
            var tree = Tree;
            if (tree == null)
                return results;

            foreach (var item in Traverse(tree.Items, span.Start.Position, span.End.Position))
            {
                var s = new Span(item.Start, item.Length);
                if (span.IntersectsWith(s))
                {
                    var type = ClassifierContextCache.Get(item.ClassifierType).GetClassification(Registry);
                    if (type != null)
                        results.Add(new ClassificationSpan(new SnapshotSpan(tree.SourceText, s), type));
                }
            }

            return results;
        }

        private void OnTreeChanged(object sender, TreeChangedEventArgs e)
        {
            Tree = e.Tree;

            var handler = ClassificationChanged;
            if (handler != null)
                handler(this, new ClassificationChangedEventArgs(new SnapshotSpan(e.Tree.SourceText, new Span(e.ChangeStart, e.ChangeEnd - e.ChangeStart))));
        }

        private IEnumerable<ParseItem> Traverse(ParseItemList items, int start, int end)
        {
            foreach (var item in items)
            {
                if (item.Start <= end && item.End >= start)
                {
                    if (item is ComplexItem)
                    {
                        foreach (var child in Traverse((item as ComplexItem).Children, start, end))
                            yield return child;
                    }
                    else
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
