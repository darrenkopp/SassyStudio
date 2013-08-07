using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using SassyStudio.Compiler;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor
{
    class SassEditorDocument
    {
        readonly object locker = new object();
        static readonly Lazy<IParserFactory> ParserFactory = new Lazy<IParserFactory>(() => SassyStudioPackage.Instance.Composition.GetExportedValue<IParserFactory>(), true);
        readonly ITextBuffer Buffer;
        readonly IParser Parser;
        readonly FileInfo SourceFile;

        public SassEditorDocument(ITextBuffer buffer)
        {
            Parser = ParserFactory.Value.Create();

            ITextDocument document;
            if (buffer.Properties.TryGetProperty(typeof(ITextDocument), out document))
                SourceFile = new FileInfo(document.FilePath);

            Buffer = buffer;
            Buffer.ChangedLowPriority += OnBufferChanged;

            Process(Buffer.CurrentSnapshot, new SingleTextChange(0, 0, Buffer.CurrentSnapshot.Length));
        }

        public event EventHandler<TreeChangedEventArgs> TreeChanged;

        public static SassEditorDocument CreateFrom(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new SassEditorDocument(buffer));
        }

        public ISassDocumentTree Tree { get; private set; }

        private async Task Process(ITextSnapshot snapshot, SingleTextChange change)
        {
            var previous = Tree;

            var context = new ParsingExecutionContext(new BufferSnapshotChangedCancellationToken(Buffer, snapshot));
            var items = await Parser.ParseAsync(new SnapshotTextProvider(snapshot), context);

            var current = new SassDocumentTree(snapshot, items);
            if (!context.IsCancellationRequested)
            {
                ReplaceTree(current);
                RaiseTreeUpdated(previous, current, change);
                var handler = TreeChanged;
                if (handler != null)
                {
                    var start = Math.Max(0, change.Position - change.DeletedLength);
                    var end = Math.Min(change.Position + change.InsertedLength, snapshot.Length);

                    handler(this, new TreeChangedEventArgs(current, start, end));
                }
            }
        }

        private void RaiseTreeUpdated(ISassDocumentTree previous, ISassDocumentTree current, SingleTextChange change)
        {
            int start = 0;
            int end = Math.Min(change.Position + change.InsertedLength, current.SourceText.Length);
            if (previous != null)
            {
                // we need to scan both trees until we find where they start lining up again
                var offset = change.InsertedLength + (-1 * change.DeletedLength);
                var original = previous.Items.FindItemContainingPosition(change.Position - change.DeletedLength);
                var updated = current.Items.FindItemContainingPosition(change.Position + change.InsertedLength);

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

            // only issue event if tree hasn't changed
            if (current == Tree)
                OnTreeChanged(current, start, end);
        }

        private async void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // ignore stale event
            if (e.After != Buffer.CurrentSnapshot) return;

            var change = CreateSingleChange(e.After, e.Changes);
            await Process(e.After, change);
        }

        private SingleTextChange CreateSingleChange(ITextSnapshot snapshot, INormalizedTextChangeCollection changes)
        {
            if (changes == null || changes.Count == 0)
                return new SingleTextChange(0, 0, snapshot.Length);

            var start = changes[0];
            var end = changes[changes.Count - 1];

            return new SingleTextChange(start.OldPosition, end.OldEnd - start.OldPosition, end.NewEnd - start.OldPosition);
        }

        private void ReplaceTree(ISassDocumentTree current)
        {
            lock (locker)
            {
                Tree = current;
            }
        }

        private void OnTreeChanged(ISassDocumentTree current, int start, int end)
        {
            var handler = TreeChanged;
            if (handler != null)
                handler(this, new TreeChangedEventArgs(current, start, end));
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

        struct TreeChanges
        {
            public readonly int Start;
            public readonly int End;

            public TreeChanges(int start, int end)
            {
                Start = start;
                End = end;
            }
        }
    }
}
