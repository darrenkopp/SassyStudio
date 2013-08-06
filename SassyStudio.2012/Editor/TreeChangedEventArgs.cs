using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Editor
{
    class TreeChangedEventArgs : EventArgs
    {
        public TreeChangedEventArgs(ISassDocumentTree tree, int changeStart, int changeEnd)
        {
            Tree = tree;
            ChangeStart = changeStart;
            ChangeEnd = changeEnd;
        }

        public ISassDocumentTree Tree { get; private set; }
        public int ChangeStart { get; private set; }
        public int ChangeEnd { get; private set; }
    }
}
