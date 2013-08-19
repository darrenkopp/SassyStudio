using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio
{
    public class FileTextManager : ITextManager
    {
        readonly FileInfo Source;
        public FileTextManager(FileInfo source)
        {
            Source = source;
        }

        public ITextProviderScope Open()
        {
            return new InMemoryTextScope(Source);
        }

        class InMemoryTextScope : ITextProviderScope
        {
            readonly string FileText;

            public InMemoryTextScope(FileInfo source)
            {
                using (var stream = source.OpenRead())
                using (var reader = new StreamReader(stream, true))
                    FileText = reader.ReadToEnd();

                Text = new StringTextProvider(FileText);
            }

            public ITextProvider Text { get; private set; }

            public void Dispose()
            {
                Text = null;
            }
        }

        class StringTextProvider : ITextProvider
        {
            readonly string Text;
            public StringTextProvider(string value)
            {
                Text = value ?? "";
            }

            public int Length { get { return Text.Length; } }

            public char this[int index]
            {
                get { return Text[index]; }
            }

            public string GetText(int start, int length)
            {
                return Text.Substring(start, length);
            }

            public bool CompareOrdinal(int start, string value)
            {
                for (int i = 0; i < value.Length; i++)
                    if (Text[start + i] != value[i])
                        return false;

                return true;
            }

            public bool StartsWithOrdinal(int start, string value)
            {
                return CompareOrdinal(start, value);
            }
        }
    }
}
