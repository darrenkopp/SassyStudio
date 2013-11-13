using SassyStudio.Compiler.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionValueProvider))]
    class ImportFileCompletionProvider : ICompletionValueProvider
    {
        public IEnumerable<ICompletionValue> GetCompletions(SassCompletionContextType type, ICompletionContext context)
        {
            if (type != SassCompletionContextType.ImportDirectiveFile)
                return Enumerable.Empty<ICompletionValue>();

            var directory = Resolve(context);
            if (directory == null)
                return Enumerable.Empty<ICompletionValue>();

            return GetValues(directory);
        }

        static DirectoryInfo Resolve(ICompletionContext context)
        {
            var path = context.DocumentTextProvider.GetText(context.Current.Start, context.Current.Length);

            var segments = path.Trim('"').Split('/');
            var directory = context.Document.Source.Directory;

            for (int i = 0; i < segments.Length - 1; i++)
            {
                // make sure we still have directory
                if (directory == null)
                    return null;

                // handle parent navigation
                var segment = segments[i];
                if (segment == "..")
                {
                    directory = directory.Parent;
                    continue;
                }

                var candidates = directory.GetDirectories(segments[i]);
                if (candidates.Length == 0)
                    return null;

                directory = candidates[0];
            }

            return directory;
        }

        static IEnumerable<ICompletionValue> GetValues(DirectoryInfo directory)
        {
            yield return new FileSystemCompletionValue("../", "../", false);

            foreach (var file in GetFiles(directory.GetFiles("*.scss")))
                yield return file;

            foreach (var d in directory.GetDirectories())
                yield return new FileSystemCompletionValue(d.Name, d.Name + "/", false);
        }

        private static IEnumerable<ICompletionValue> GetFiles(FileInfo[] files)
        {
            return files
                .Select(x => x.Name.StartsWith("_") ? x.Name.Substring(1) : x.Name)
                .Select(x => new FileSystemCompletionValue(
                    displayText: x,
                    completionText: Path.GetFileNameWithoutExtension(x),
                    closeValue: true
                ));
        }

        public IEnumerable<SassCompletionContextType> SupportedContexts
        {
            get { yield return SassCompletionContextType.ImportDirectiveFile; }
        }
    }
}
