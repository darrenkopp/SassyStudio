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

        public event System.EventHandler<StylesheetChangedEventArgs> StylesheetChanged;

        public FileInfo Source { get; private set; }
        public ISassStylesheet Stylesheet { get; private set; }

        public ISassStylesheet Update(ISassStylesheet stylesheet)
        {
            lock (locker)
            {
                var previous = Stylesheet;
                Stylesheet = stylesheet;

                return previous;
            }
        }
    }
}