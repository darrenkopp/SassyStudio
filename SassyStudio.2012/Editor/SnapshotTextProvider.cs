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

        public string GetText(int start, int length)
        {
            length = Math.Min(length, Length - start);

            return Snapshot.GetText(start, length);
        }

        public bool CompareOrdinal(int start, string value)
        {
            var text = GetText(start, value.Length);
            if (text == value)
                return true;

            return false;
        }
    }
}
