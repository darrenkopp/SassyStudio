using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using SassyStudio.Compiler.Parsing;
using SassyStudio.Scss;

namespace SassyStudio.Editor
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    class SassOutliningTaggerProvider : ITaggerProvider
    {
        [Import]
        internal IParserFactory ParserFactory { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (typeof(T) == typeof(IOutliningRegionTag))
                return buffer.Properties.GetOrCreateSingletonProperty(() => new SassOutliningTagger(buffer, ParserFactory)) as ITagger<T>;

            return null;
        }
    }

    class SassOutliningTagger : ITagger<IOutliningRegionTag>
    {
        readonly SassEditorDocument Editor;
        public SassOutliningTagger(ITextBuffer buffer, IParserFactory parserFactory)
        {
            Editor = SassEditorDocument.CreateFrom(buffer, parserFactory);
            Editor.TreeChanged += OnTreeChanged;
        }

        private ISassDocumentTree Tree { get; set; }

        private void OnTreeChanged(object sender, TreeChangedEventArgs e)
        {
            Tree = e.Tree;
            var handler = TagsChanged;
            if (Tree != null && handler != null)
                handler(this, new SnapshotSpanEventArgs(new SnapshotSpan(Tree.SourceText, 0, Tree.SourceText.Length)));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var tree = Tree;
            if (spans.Count == 0 || tree == null) return new List<ITagSpan<IOutliningRegionTag>>(0);

            var results = new SortedList<int, ITagSpan<IOutliningRegionTag>>();
            foreach (var block in GetBlocks(tree.Items, spans[0].Start, spans[spans.Count - 1].End))
            {
                var open = block.OpenCurlyBrace;
                var close = block.CloseCurlyBrace;

                if (open != null && close != null)
                {
                    var startLine = tree.SourceText.GetLineFromPosition(open.Start);
                    var endLine = tree.SourceText.GetLineFromPosition(close.Start);
                    if (startLine.LineNumber != endLine.LineNumber)
                    {
                        var span = new SnapshotSpan(tree.SourceText, new Span(open.End, close.Start - open.End));
                        var tag = new OutliningRegionTag(false, false, "...", "...");

                        results.Add(span.Start.Position, new TagSpan<IOutliningRegionTag>(span, tag));
                    }
                }
            }

            return results.Values;
        }

        IEnumerable<BlockItem> GetBlocks(ParseItemList items, int start, int end)
        {
            foreach (var complex in items.OfType<ComplexItem>())
            {
                if (complex.Start <= end && complex.End >= start)
                {
                    foreach (var child in GetBlocks(complex.Children, start, end))
                        yield return child;

                    if (complex is BlockItem)
                        yield return complex as BlockItem;
                }
            }
        }
    }
}
