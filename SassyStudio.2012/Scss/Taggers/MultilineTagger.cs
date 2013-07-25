using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using SassyStudio.Scss.Classifications;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Scss.Taggers
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IClassificationTag))]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    class MultilineTaggerProvider : ITaggerProvider
    {
        [Import, System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal IClassificationTypeRegistryService Registry { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return buffer.Properties.GetOrCreateSingletonProperty<MultilineTagger>(() => new MultilineTagger(buffer, Registry.GetClassificationType(ScssClassificationTypes.Comment))) as ITagger<T>;
        }
    }

    class MultilineTagger : ITagger<ClassificationTag>
    {
        readonly ITextBuffer Buffer;
        readonly ClassificationTag Tag;

        public MultilineTagger(ITextBuffer buffer, IClassificationType commentType)
        {
            Tag = new ClassificationTag(commentType);
            CurrentSnapshot = buffer.CurrentSnapshot;
            CurrentComments = new List<CommentRegion>();
            Buffer = buffer;
            Buffer.ChangedLowPriority += OnBufferChanged;
            
            Task.Run(() => this.Parse());
        }

        private ITextSnapshot CurrentSnapshot { get; set; }
        private IReadOnlyCollection<CommentRegion> CurrentComments { get; set; }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0) yield break;

            var snapshot = CurrentSnapshot;
            var comments = CurrentComments;

            var startLine = spans[0].Start.GetContainingLine().LineNumber;
            var endLine = spans[spans.Count - 1].End.GetContainingLine().LineNumber;

            foreach (var region in comments.Where(x => x.StartLine <= endLine && x.EndLine >= startLine))
            {
                var span = AsSnapshotSpan(region, snapshot);

                yield return new TagSpan<ClassificationTag>(span, Tag);
            }
        }

        private void Parse()
        {
            var snapshot = Buffer.CurrentSnapshot;
            var comments = GetComments(snapshot);

            var originalSpans = GetSpans(CurrentComments, CurrentSnapshot, snapshot);
            var currentSpans = GetSpans(comments, snapshot, snapshot);

            // figure out who has changed (added or removed)
            var unchanged = originalSpans.Intersect(currentSpans).ToList();
            var added = currentSpans.Except(unchanged);
            var removed = originalSpans.Except(unchanged);

            // get normalized view or the range of what has ben added / removed
            var changed = new NormalizedSnapshotSpanCollection(
                added.Concat(removed).Select(span => new SnapshotSpan(snapshot, span))
            );

            // update our regions and snapshots
            CurrentSnapshot = snapshot;
            CurrentComments = comments;

            var tagsChanged = this.TagsChanged;
            if (tagsChanged != null && changed.Count > 0)
            {
                foreach (var span in changed)
                    tagsChanged(this, new SnapshotSpanEventArgs(span));
            }
        }

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event).
            if (e.After != Buffer.CurrentSnapshot) return;

            Task.Run(() => Parse());
        }

        static IReadOnlyCollection<CommentRegion> GetComments(ITextSnapshot snapshot)
        {
            var results = new List<CommentRegion>();

            CommentRegion current = null;
            foreach (var line in snapshot.Lines)
            {
                var text = line.GetText();
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    if (c == '/')
                    {
                        // already in comment, this won't break us out until we get a *
                        if (current != null) continue;
                        // eager scan ahead by 1+ character (if not a match, won't matter anyway
                        if ((i + 1) < text.Length)
                        {
                            char n = text[++i];
                            // double forward slash means we can stop attempting to parse
                            if (n == '/') break;
                            if (n == '*')
                            {
                                current = new CommentRegion
                                {
                                    StartLine = line.LineNumber,
                                    StartOffset = i-1 // offset the ++
                                };
                            }
                        }
                    }
                    else if (c == '*')
                    {
                        // not in a comment, this won't matter
                        if (current == null) continue;
                        if ((i + 1) < text.Length)
                        {
                            char n = text[++i];
                            if (n == '/')
                            {
                                // capture ending information, add to results and reset
                                current.EndLine = line.LineNumber;
                                current.EndOffset = i - 1; // offset the ++
                                results.Add(current);
                                current = null;
                            }
                        }
                    }
                }
            }

            if (current != null)
            {
                var lastLine = snapshot.Lines.Last();
                current.EndLine = lastLine.LineNumber;
                current.EndOffset = lastLine.End.Position;
                results.Add(current);
                current = null;
            }

            return results;
        }

        private static IEnumerable<Span> GetSpans(IEnumerable<CommentRegion> regions, ITextSnapshot originalSnapshot, ITextSnapshot currentSnapshot)
        {
            var results = regions.Select(r => AsSnapshotSpan(r, originalSnapshot));
            if (originalSnapshot != currentSnapshot)
                results = results.Select(s => s.TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive));

            return results.Select(x => x.Span);
        }

        static SnapshotSpan AsSnapshotSpan(CommentRegion region, ITextSnapshot snapshot)
        {
            var startLine = snapshot.GetLineFromLineNumber(region.StartLine);
            var endLine = snapshot.GetLineFromLineNumber(region.EndLine);
            return new SnapshotSpan(startLine.Start + region.StartOffset, endLine.End + region.EndOffset);
        }


        class CommentRegion
        {
            public int StartLine { get; set; }
            public int StartOffset { get; set; }
            public int EndLine { get; set; }
            public int EndOffset { get; set; }
        }
    }
}
