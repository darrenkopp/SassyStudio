using System;

namespace SassyStudio
{
    public class StylesheetChangedEventArgs : EventArgs
    {
        public StylesheetChangedEventArgs(ISassStylesheet previous, ISassStylesheet current)
        {
            Previous = previous;
            Current = current;
        }

        public ISassStylesheet Current { get; set; }
        public ISassStylesheet Previous { get; set; }
    }
}