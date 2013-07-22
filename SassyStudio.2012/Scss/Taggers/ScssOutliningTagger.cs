using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace SassyStudio.Scss.Taggers
{
    class ScssOutliningTagger : ITagger<IOutliningRegionTag>
    {
        const string ellipsis = "...";    //the characters that are displayed when the region is collapsed
        readonly ITextBuffer Buffer;

        public ScssOutliningTagger(ITextBuffer buffer)
        {
            Buffer = buffer;
            CurrentSnapshot = buffer.CurrentSnapshot;
            CurrentRegions = new List<Region>(0);

            Buffer.ChangedLowPriority += OnBufferChanged;

            // run parsing
            Task.Run(() => this.ReParse());
        }

        private ITextSnapshot CurrentSnapshot { get; set; }
        private IReadOnlyCollection<Region> CurrentRegions { get; set; }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0) yield break;

            var snapshot = CurrentSnapshot;
            var regions = CurrentRegions;

            var startLine = spans[0].Start.GetContainingLine().LineNumber;
            var endLine = spans[spans.Count - 1].End.GetContainingLine().LineNumber;

            foreach (var region in regions.Where(x => x.StartLine <= endLine && x.EndLine >= startLine))
            {
                var span = AsSnapshotSpan(region, snapshot);
                var tag = new OutliningRegionTag(false, false, ellipsis, span.GetText());

                yield return new TagSpan<IOutliningRegionTag>(span, tag);
            }
        }

        void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event).
            if (e.After != Buffer.CurrentSnapshot) return;

            Task.Run(() => ReParse());
        }

        void ReParse()
        {
            var snapshot = Buffer.CurrentSnapshot;
            var regions = GetRegions(snapshot);

            var originalSpans = GetSpans(CurrentRegions, CurrentSnapshot, snapshot);
            var currentSpans = GetSpans(regions, snapshot, snapshot);

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
            CurrentRegions = regions;

            var tagsChanged = this.TagsChanged;
            if (tagsChanged != null && changed.Count > 0)
            {
                foreach (var span in changed)
                    tagsChanged(this, new SnapshotSpanEventArgs(span));
            }
        }

        private static IEnumerable<Span> GetSpans(IEnumerable<Region> regions, ITextSnapshot originalSnapshot, ITextSnapshot currentSnapshot)
        {
            var results = regions.Select(r => AsSnapshotSpan(r, originalSnapshot));
            if (originalSnapshot != currentSnapshot)
                results = results.Select(s => s.TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive));

            return results.Select(x => x.Span);
        }

        static IReadOnlyCollection<Region> GetRegions(ITextSnapshot snapshot)
        {
            List<Region> results = new List<Region>();

            Stack<Region> regions = new Stack<Region>();
            foreach (var line in snapshot.Lines)
            {
                var text = line.GetText();
                for (int i = 0; i < text.Length; i++)
                {
                    char current = text[i];
                    if (current == '{')
                    {
                        // start a new region
                        regions.Push(new Region
                        {
                            StartLine = line.LineNumber,
                            StartOffset = i
                        });
                    }
                    else if (current == '}')
                    {
                        // if we have opening region
                        if (regions.Count > 0)
                        {
                            var region = regions.Pop();
                            region.EndLine = line.LineNumber;

                            // don't outline if open and close on same line
                            if (region.StartLine < region.EndLine)
                                results.Add(region);
                        }
                    }
                }
            }

            // pop off stranded regions until we get to the start
            while (regions.Count > 1)
                regions.Pop();

            // if we have an unclosed region, close it with ending line
            if (regions.Count > 0)
            {
                var lastRegion = regions.Pop();
                lastRegion.EndLine = snapshot.Lines.Last().LineNumber;
                if (lastRegion.StartLine < lastRegion.EndLine)
                    results.Add(lastRegion);
            }

            return results;
        }

        static SnapshotSpan AsSnapshotSpan(Region region, ITextSnapshot snapshot)
        {
            var startLine = snapshot.GetLineFromLineNumber(region.StartLine);
            var endLine = snapshot.GetLineFromLineNumber(region.EndLine);
            return new SnapshotSpan(startLine.Start + region.StartOffset, endLine.End);
        }

        class Region
        {
            public int StartLine { get; set; }
            public int StartOffset { get; set; }
            public int EndLine { get; set; }
        }
    }
}
