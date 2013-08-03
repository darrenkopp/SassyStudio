using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Editor
{
    class TreeChangedEventArgs : EventArgs
    {
        private readonly ISassDocumentTree _Tree;
        public TreeChangedEventArgs(ISassDocumentTree tree)
        {
            _Tree = tree;
        }

        public ISassDocumentTree Tree { get { return _Tree; } }
    }
}
