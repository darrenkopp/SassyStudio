using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using SassyStudio.Compiler;

namespace SassyStudio.Editor
{
    struct BufferSnapshotChangedCancellationToken : IParsingCancellationToken
    {
        private readonly ITextBuffer Buffer;
        private readonly ITextSnapshot Snapshot;
        public BufferSnapshotChangedCancellationToken(ITextBuffer buffer, ITextSnapshot snapshot)
        {
            Buffer = buffer;
            Snapshot = snapshot;
        }

        public bool IsCancellationRequested { get { return Buffer.CurrentSnapshot != Snapshot; } }
    }
}
