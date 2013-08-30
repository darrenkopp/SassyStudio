using System;
using System.IO;

namespace SassyStudio
{
    public class SassDocument : ISassDocument
    {
        readonly object locker = new object();
        public SassDocument(FileInfo source)
        {
            Source = source;
        }

        public event EventHandler<StylesheetChangedEventArgs> StylesheetChanged;

        public FileInfo Source { get; private set; }
        public ISassStylesheet Stylesheet { get; private set; }

        public ISassStylesheet Update(ISassStylesheet stylesheet)
        {
            lock (locker)
            {
                var previous = Stylesheet;
                Stylesheet = stylesheet;

                OnStylesheetChanged(previous, stylesheet);

                return previous;
            }
        }

        private void OnStylesheetChanged(ISassStylesheet previous, ISassStylesheet current)
        {
            var handler = StylesheetChanged;
            if (handler != null)
                handler(this, new StylesheetChangedEventArgs(previous, current));
        }
    }
}