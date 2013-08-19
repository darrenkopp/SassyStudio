using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio
{
    [Export(typeof(IDocumentManager))]
    class DocumentManager : IDocumentManager
    {
        readonly ConcurrentDictionary<FileInfo, ISassDocument> Cache = new ConcurrentDictionary<FileInfo, ISassDocument>(new FileComparer());

        public event EventHandler<DocumentAddedEventArgs> DocumentAdded;

        public ISassDocument Get(FileInfo source)
        {
            return Cache.GetOrAdd(source, key => Create(key));
        }

        public ISassDocument Import(FileInfo source, ISassDocument owner)
        {
            return Cache.GetOrAdd(source, key => Create(key));
        }

        ISassDocument Create(FileInfo source)
        {
            var document = new SassDocument(source);

            var handler = DocumentAdded;
            if (handler != null)
                handler(this, new DocumentAddedEventArgs(document));

            return document;
        }

        class FileComparer : IEqualityComparer<FileInfo>
        {
            readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;
            public bool Equals(FileInfo x, FileInfo y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                return Comparer.Equals(x.FullName, y.FullName);
            }

            public int GetHashCode(FileInfo obj)
            {
                return Comparer.GetHashCode(obj.FullName);
            }
        }
    }
}
