using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class ImportFile : ComplexItem
    {
        public ISassDocument Document { get; set; }
        public StringValue Filename { get; protected set; }
        public TokenItem Comma { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.String || stream.Current.Type == TokenType.BadString)
            {
                Filename = itemFactory.CreateSpecificParsed<StringValue>(this, text, stream);
                if (Filename != null)
                    Children.Add(Filename);

                if (stream.Current.Type == TokenType.Comma)
                    Comma = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);
            }

            return Children.Count > 0;
        }

        public string ResolvePath(DirectoryInfo currentDirectory, ITextProvider text)
        {
            if (Filename == null || Filename.Length == 0)
                return null;

            var relativePath = text.GetText(Filename.Start, Filename.Length).Trim('\'', '"');
            var segments = relativePath.Split('/');
            if (segments.Length == 0)
                return null;

            var path = currentDirectory.FullName;
            for (int i = 0; i < (segments.Length - 1); i++)
                path = Path.Combine(path, segments[i]);

            var directory = new DirectoryInfo(Path.GetFullPath(path));
            var filename = segments[segments.Length - 1];
            if (string.IsNullOrEmpty(Path.GetExtension(filename)))
                filename += ".scss";

            var files = directory.GetFiles("*" + filename);
            
            var comparer = StringComparer.OrdinalIgnoreCase;
            return files.Where(x => comparer.Equals(x.Name, filename) || comparer.Equals(x.Name, "_" + filename))
                .Select(x => x.FullName)
                .FirstOrDefault();
        }
    }
}
