using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using SassyStudio.Compiler;
using SassyStudio.Compiler.Parsing;
using SassyStudio.Editor.Intellisense;

namespace SassyStudio.Editor
{
    class SassEditor : ISassEditor
    {
        readonly ITextBuffer Buffer;
        readonly IForegroundParsingTask ParsingTask;
        readonly ISassDocument _Document;

        public SassEditor(ITextBuffer buffer, ISassDocument document, IForegroundParsingTask parsingTask)
        {
            Buffer = buffer;
            _Document = document;
            ParsingTask = parsingTask;

            Buffer.ChangedLowPriority += OnBufferChanged;
        }

        public event EventHandler<DocumentChangedEventArgs> DocumentChanged;
        public ISassDocument Document { get { return _Document; } }

        public ICompletionContext CreateCompletionContext(int position)
        {
            throw new NotImplementedException();
        }

        void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // buffer is stale, ignore because we will get another
            if (e.After != Buffer.CurrentSnapshot) return;

            Task.Run(() => Process(e.After, CreateSingleChange(e.After, e.Changes)));
        }

        void Process(ITextSnapshot snapshot, SingleTextChange change)
        {
            var stylesheet = ParsingTask.Parse(new TextSnapshotParsingRequest(Buffer, Document));
            if (stylesheet != null)
            {
                var previous = Document.Update(stylesheet);

                var handler = DocumentChanged;
                if (handler != null)
                    handler(this, ComputeChanges(previous, stylesheet, snapshot, change));
            }
        }

        private SingleTextChange CreateSingleChange(ITextSnapshot snapshot, INormalizedTextChangeCollection changes)
        {
            if (changes == null || changes.Count == 0)
                return new SingleTextChange(0, 0, snapshot.Length);

            var start = changes[0];
            var end = changes[changes.Count - 1];

            return new SingleTextChange(start.OldPosition, end.OldEnd - start.OldPosition, end.NewEnd - start.OldPosition);
        }

        DocumentChangedEventArgs ComputeChanges(ISassStylesheet previous, ISassStylesheet current, ITextSnapshot snapshot, SingleTextChange change)
        {
            if (previous == null)
                return new DocumentChangedEventArgs { Stylesheet = current, ChangeStart = 0, ChangeEnd = snapshot.Length };

            int start = 0;
            int end = Math.Min(change.Position + change.InsertedLength, snapshot.Length);
            if (previous != null)
            {
                // we need to scan both trees until we find where they start lining up again
                var offset = change.InsertedLength + (-1 * change.DeletedLength);
                var original = previous.Children.FindItemContainingPosition(change.Position - change.DeletedLength) ?? previous as Stylesheet;
                var updated = current.Children.FindItemContainingPosition(change.Position + change.InsertedLength) ?? current as Stylesheet;

                if (original != null && updated != null)
                    start = updated.Start;

                while (true)
                {
                    if (original == null || updated == null)
                        break;

                    // update our positions
                    start = Math.Min(start, updated.Start);
                    end = Math.Max(end, updated.End);

                    if (original.GetType() == updated.GetType())
                    {
                        // there are two types of changes (adding characters or removing characters)
                        // if we added characters then we'll need to adjust end characters
                        // if we deleted characters then we'll need to offset starting characters OR ending characters

                        // checking for length being extended by adding characters
                        if (original.Start == updated.Start && (original.End + change.InsertedLength) == updated.End)
                        {
                            end = updated.End;
                            break;
                        }
                        // checking for length being shortened by deleting characters
                        else if (original.Start == updated.Start && (original.End - change.DeletedLength) == updated.End)
                        {
                            break;
                        }
                        // checking for removal of nodes?
                        else if (original.Start == (updated.Start - change.DeletedLength) && (original.End - change.DeletedLength) == updated.End)
                        {
                            break;
                        }
                    }

                    original = original.InOrderSuccessor();
                    updated = updated.InOrderSuccessor();
                }
            }

            return new DocumentChangedEventArgs
            {
                Stylesheet = current,
                ChangeStart = start,
                ChangeEnd = end
            };
        }

        class TextSnapshotParsingRequest : IParsingRequest
        {
            readonly ITextBuffer Buffer;
            readonly ITextSnapshot Snapshot;

            public TextSnapshotParsingRequest(ITextBuffer buffer, ISassDocument document)
            {
                Buffer = buffer;
                Snapshot = Buffer.CurrentSnapshot;
                Document = document;

                Text = new SnapshotTextProvider(Snapshot);
            }

            public ISassDocument Document { get; private set; }
            public ITextProvider Text { get; private set; }
            public DateTime RequestedOn { get; private set; }
            public bool IsCancelled { get { return Snapshot != Buffer.CurrentSnapshot; } }
        }

        struct SingleTextChange
        {
            readonly int _Position;
            readonly int _DeletedLength;
            readonly int _InsertedLength;
            public SingleTextChange(int position, int deleted, int inserted)
            {
                _Position = position;
                _DeletedLength = deleted;
                _InsertedLength = inserted;
            }

            public int Position { get { return _Position; } }
            public int DeletedLength { get { return _DeletedLength; } }
            public int InsertedLength { get { return _InsertedLength; } }
        }
    }
}
