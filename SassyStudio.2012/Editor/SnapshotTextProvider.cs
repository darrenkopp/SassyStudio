using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using SassyStudio.Compiler;

namespace SassyStudio.Editor
{
    class SnapshotTextProvider : ITextProvider
    {
        private readonly ITextSnapshot Snapshot;
        public SnapshotTextProvider(ITextSnapshot snapshot)
        {
            Snapshot = snapshot;
        }

        public int Length { get { return Snapshot.Length; } }

        public char this[int index] { get { return Snapshot[index]; } }
    }
}
