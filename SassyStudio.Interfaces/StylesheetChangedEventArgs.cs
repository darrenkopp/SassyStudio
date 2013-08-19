using System;

namespace SassyStudio
{
    public class StylesheetChangedEventArgs : EventArgs
    {
        public ISassStylesheet Current { get; set; }
        public ISassStylesheet Previous { get; set; }
        public int ChangeStart { get; set; }
        public int ChangeEnd { get; set; }
    }
}