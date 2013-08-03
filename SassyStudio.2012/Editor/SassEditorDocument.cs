using System;
using System.Collections.Generic;
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
        readonly ITextBuffer Buffer;
        readonly IParser Parser;
        readonly FileInfo SourceFile;

        public SassEditorDocument(ITextBuffer buffer, FileInfo sourceFile, IParser parser)
        {
            Parser = parser;
            SourceFile = sourceFile;
            Buffer = buffer;
            Buffer.ChangedLowPriority += OnBufferChanged;

            Task.Run(() => Initialize(Buffer.CurrentSnapshot));
        }

        private ISassDocumentTree Tree { get; set; }

        private async Task Initialize(ITextSnapshot snapshot)
        {
            try
            {
                var tree = await Parse(snapshot);

                ReplaceTree(tree);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public event EventHandler<TreeChangedEventArgs> TreeChanged;

        private async void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // ignore stale event
            if (e.After != Buffer.CurrentSnapshot) return;

            // parse the new tree
            var current = await Parse(e.After);
            if (e.After == Buffer.CurrentSnapshot)
                ReplaceTree(current);
        }

        private void ReplaceTree(ISassDocumentTree current)
        {
            var original = Tree;
            // TODO: check to see if anything has changed

            Tree = current;
            OnTreeChanged(original, current);
        }

        private async Task<ISassDocumentTree> Parse(ITextSnapshot snapshot)
        {
            try
            {
                var context = new ParsingExecutionContext(new BufferSnapshotChangedCancellationToken(Buffer, snapshot));
                var items = await Parser.ParseAsync(new SnapshotTextProvider(snapshot), context);

                var tree = new SassDocumentTree(snapshot, items);
                return tree;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }

        private void OnTreeChanged(ISassDocumentTree original, ISassDocumentTree current)
        {
            var handler = TreeChanged;
            if (handler != null)
            {
                handler(this, new TreeChangedEventArgs(current));
            }
        }
    }
}
