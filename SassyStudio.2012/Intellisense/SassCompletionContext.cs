using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using SassyStudio.Compiler.Parsing;
using SassyStudio.Editor;

namespace SassyStudio.Intellisense
{
    struct SassCompletionContext
    {
        readonly ITextProvider _Text;
        readonly int _StartPosition;
        readonly ITrackingSpan _TrackingSpan;
        readonly ParseItem _Current;
        readonly IEnumerable<ComplexItem> _TraversalPath;

        public SassCompletionContext(ITextSnapshot snapshot, ITrackingSpan trackingSpan, ParseItem current, IEnumerable<ComplexItem> path)
        {
            _Text = new SnapshotTextProvider(snapshot);
            _StartPosition = trackingSpan.GetStartPoint(snapshot).Position;
            _TrackingSpan = trackingSpan;
            _Current = current;
            _TraversalPath = path;
        }

        public ITextProvider Text { get { return _Text; } }
        public int StartPosition { get { return _StartPosition; } }
        public ITrackingSpan TrackingSpan { get { return _TrackingSpan; } }
        public ParseItem Current { get { return _Current; } }
        public IEnumerable<ComplexItem> TraversalPath { get { return _TraversalPath; } }
    }
}
