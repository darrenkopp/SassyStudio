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

namespace SassyStudio.Editor.Outlining
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    class ScssOutliningTaggerProvider : ITaggerProvider
    {
        [Import]
        ISassEditorManager EditorManager { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (typeof(T) == typeof(IOutliningRegionTag))
                return buffer.Properties.GetOrCreateSingletonProperty(() => Create(buffer)) as ITagger<T>;

            return null;
        }

        ScssOutliningTagger Create(ITextBuffer buffer)
        {
            var editor = EditorManager.Get(buffer);
            if (editor != null)
                return new ScssOutliningTagger(buffer, editor);

            return null;
        }
    }

    class ScssOutliningTagger : ITagger<IOutliningRegionTag>
    {
        readonly ITextBuffer Buffer;
        readonly ISassEditor Editor;

        public ScssOutliningTagger(ITextBuffer buffer, ISassEditor editor)
        {
            Buffer = buffer;
            Editor = editor;

            Editor.DocumentChanged += OnDocumentChanged;
        }

        void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            var handler = TagsChanged;
            if (handler != null)
                handler(this, new SnapshotSpanEventArgs(new SnapshotSpan(Buffer.CurrentSnapshot, new Span(e.ChangeStart, e.ChangeEnd))));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var stylesheet = Editor.Document.Stylesheet;
            var snapshot = Buffer.CurrentSnapshot;
            if (spans.Count == 0 || stylesheet == null) return new List<ITagSpan<IOutliningRegionTag>>(0);

            var results = new SortedList<int, ITagSpan<IOutliningRegionTag>>();
            foreach (var block in GetBlocks(new ParseItemList { stylesheet as Stylesheet }, spans[0].Start, spans[spans.Count - 1].End))
            {
                var open = block.OpenCurlyBrace;
                var close = block.CloseCurlyBrace;

                if (open != null && close != null)
                {
                    var startLine = snapshot.GetLineFromPosition(open.Start);
                    var endLine = snapshot.GetLineFromPosition(close.Start);
                    if (startLine.LineNumber != endLine.LineNumber)
                    {
                        var span = new SnapshotSpan(snapshot, new Span(open.End, close.Start - open.End));
                        var tag = new OutliningRegionTag(false, false, "...", "...");

                        results.Add(span.Start.Position, new TagSpan<IOutliningRegionTag>(span, tag));
                    }
                }
            }

            return results.Values;
        }

        IEnumerable<BlockItem> GetBlocks(ParseItemList items, int start, int end)
        {
            foreach (var item in items)
            {
                if (item.Start <= end && item.End >= start)
                {
                    var container = item as ComplexItem;
                    if (container != null)
                    {
                        if (container is BlockItem)
                            yield return container as BlockItem;

                        foreach (var child in GetBlocks(container.Children, start, end))
                            yield return child;
                    }
                }
            }
        }
    }
}
