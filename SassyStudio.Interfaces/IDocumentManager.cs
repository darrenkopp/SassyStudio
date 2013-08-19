using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio
{
    public interface IDocumentManager
    {
        event EventHandler<DocumentAddedEventArgs> DocumentAdded;
        ISassDocument Get(FileInfo source);
        ISassDocument Import(FileInfo source, ISassDocument owner);
    }

    public class DocumentAddedEventArgs : EventArgs
    {
        public DocumentAddedEventArgs(ISassDocument document)
        {
            Document = document;
        }

        public ISassDocument Document { get; private set; }
    }
}
