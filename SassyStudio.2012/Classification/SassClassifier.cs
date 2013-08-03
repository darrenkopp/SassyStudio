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

        public SassClassifier(IClassificationTypeRegistryService registry, SassEditorDocument editor)
        {
            Registry = registry;
            editor.TreeChanged += OnTreeChanged;
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
                    var classificationType = ClassifierContextCache.Get(item.ClassifierType).GetClassification(Registry);
                    if (classificationType != null)
                    {
                        if (classificationType.Classification.Contains("string"))
                            Logger.Log(tree.SourceText.GetText(s));

                        results.Add(new ClassificationSpan(new SnapshotSpan(tree.SourceText, s), classificationType));
                    }
                }
            }

            return results;
        }

        private void OnTreeChanged(object sender, TreeChangedEventArgs e)
        {
            Tree = e.Tree;
            // TODO: calculate differences and update span?

            var handler = ClassificationChanged;
            if (handler != null)
                handler(this, new ClassificationChangedEventArgs(new SnapshotSpan(e.Tree.SourceText, new Span(0, e.Tree.SourceText.Length))));
        }

        private IEnumerable<ParseItem> Traverse(IEnumerable<ParseItem> items, int start, int end)
        {
            foreach (var item in items.Where(x => x.Start <= end && x.End >= start))
            {
                yield return item;                

                // depth first children
                var complex = item as ComplexItem;
                if (complex != null)
                    foreach (var child in Traverse(complex.Children, start, end))
                        yield return child;
            }
        }
    }
}
