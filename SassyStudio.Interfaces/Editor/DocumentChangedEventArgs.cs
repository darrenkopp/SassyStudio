using System;

namespace SassyStudio.Editor
{
    public class DocumentChangedEventArgs : EventArgs
    {
        public ISassStylesheet Stylesheet { get; set; }
        public int ChangeStart { get; set; }
        public int ChangeEnd { get; set; }
    }
}