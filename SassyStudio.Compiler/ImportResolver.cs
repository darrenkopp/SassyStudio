using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Compiler
{
    static class ImportResolver
    {
        public static string ResolvePath(StringValue item, ITextProvider text, DirectoryInfo currentDirectory)
        {
            var relativePath = text.GetText(item.Start, item.Length).Trim('\'', '"');
            var segments = relativePath.Split('/');
            if (segments.Length == 0)
                return null;

            var path = currentDirectory.FullName;
            for (int i = 0; i < (segments.Length - 1); i++)
                path = Path.Combine(path, segments[i]);

            var directory = new DirectoryInfo(Path.GetFullPath(path));
            if (!directory.Exists)
                return null;

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
