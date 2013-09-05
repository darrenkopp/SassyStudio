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
            //return new MemoryMappedFileTextScope(Source);
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
                length = Math.Min(length, Length - start);
                if (length > 0)
                    return Text.Substring(start, length);

                return string.Empty;
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

        //class MemoryMappedFileTextScope : ITextProviderScope
        //{
        //    readonly MemoryMappedFile File;
        //    readonly MemoryMappedFileTextProvider _Text;

        //    public MemoryMappedFileTextScope(FileInfo source)
        //    {
        //        Encoding encoding = Encoding.Default;
        //        using (var reader = new StreamReader(source.FullName, true))
        //            encoding = reader.CurrentEncoding;

        //        OutputLogger.Log("Using encoding: " + encoding.EncodingName);
        //        File = MemoryMappedFile.CreateFromFile(source.FullName, FileMode.Open);
        //        try
        //        {
        //            _Text = new MemoryMappedFileTextProvider(File, (int)source.Length, encoding);
        //        }
        //        catch
        //        {
        //            File.Dispose();
        //            throw;
        //        }
        //    }

        //    public ITextProvider Text { get { return _Text; } }

        //    public void Dispose()
        //    {
        //        using (File)
        //            _Text.Dispose();
        //    }
        //}

        //class MemoryMappedFileTextProvider : ITextProvider, IDisposable
        //{
        //    readonly MemoryMappedViewAccessor Accessor;
        //    readonly StringBuilder TextBuffer;
        //    readonly Encoding Encoding;
        //    readonly byte[] DataBuffer;
        //    readonly char[] ConversionBuffer = new char[1];

        //    public MemoryMappedFileTextProvider(MemoryMappedFile file, int length, Encoding encoding)
        //    {
        //        Length = length;
        //        TextBuffer = new StringBuilder(0);
        //        Encoding = encoding;
        //        DataBuffer = new byte[encoding.GetByteCount("a")];
        //        Accessor = file.CreateViewAccessor(0L, 0L, MemoryMappedFileAccess.Read);
        //    }

        //    public int Length { get; private set; }

        //    public char this[int index]
        //    {
        //        get
        //        {
        //            int length = Accessor.ReadArray(index, DataBuffer, 0, DataBuffer.Length);

        //            Encoding.GetChars(DataBuffer, 0, length, ConversionBuffer, 0);
        //            return ConversionBuffer[0];
        //        }
        //    }

        //    public string GetText(int start, int length)
        //    {
        //        length = Math.Min(length, Length - start);
        //        if (length > 0)
        //        {
        //            TextBuffer.Clear();
        //            for (int i = start; i < (start + length); i++)
        //                TextBuffer.Append(this[i]);

        //            return TextBuffer.ToString();
        //        }

        //        return string.Empty;
        //    }

        //    public bool CompareOrdinal(int start, string value)
        //    {
        //        var data = Encoding.GetBytes(value);
        //        for (int i = start; 
        //        for (int i = 0; i < value.Length; i++)
        //            if (this[start + i] != value[i])
        //                return false;

        //        return true;
        //    }

        //    public bool StartsWithOrdinal(int start, string value)
        //    {
        //        return CompareOrdinal(start, value);
        //    }

        //    public void Dispose()
        //    {
        //        Accessor.Dispose();
        //    }
        //}
    }
}
