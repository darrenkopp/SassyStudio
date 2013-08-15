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
            try
            {
                var tree = Tree;
                if (tree == null)
                    return results;

                var item = tree.Items.FindItemContainingPosition(span.Start);
                if (item == null)
                    return results;

                if (!(item is IParseItemContainer))
                    item = item.Parent ?? item;

                for (var current = item; current != null; current = current.InOrderSuccessor())
                {
                    if (current.Start <= span.End && current.End >= span.Start)
                    {
                        var type = ClassifierContextCache.Get(current.ClassifierType).GetClassification(Registry);
                        if (type != null)
                            results.Add(new ClassificationSpan(new SnapshotSpan(tree.SourceText, new Span(current.Start, current.Length)), type));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Failed to classify");
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
