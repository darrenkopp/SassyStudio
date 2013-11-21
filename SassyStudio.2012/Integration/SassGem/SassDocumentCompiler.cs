using System.Diagnostics;
using SassyStudio.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SassyStudio.Options;

namespace SassyStudio.Integration.SassGem
{
    class SassDocumentCompiler : IDocumentCompiler
    {
        private readonly ScssOptions Options;

        public SassDocumentCompiler(ScssOptions options)
        {
            Options = options;
        }

        public void Compile(FileInfo source, FileInfo output)
        {
            var start = new ProcessStartInfo
            {
                FileName = SassSupport.SassBatchFile,
                Arguments = GetSassArguments(source, output),
                WorkingDirectory = source.FullName,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (var executor = Process.Start(start))
            {
                executor.WaitForExit();

                if (executor.ExitCode != 0)
                    throw new Exception("Sass returned an error.");
            }
        }

        public FileInfo GetOutput(FileInfo source)
        {
            var filename = Path.GetFileNameWithoutExtension(source.Name);
            var directory = DetermineSaveDirectory(source);
            var target = new FileInfo(Path.Combine(directory.FullName, filename + ".css"));

            return target;
        }

        private string GetSassArguments(FileInfo input, FileInfo output)
        {
            return new StringBuilder()
                .Append("--no-cache ")
                .Append(@"""").Append(input.FullName).Append(@"""").Append(" ")
                .Append(@"""").Append(output.FullName).Append(@"""")
            .ToString();
        }

        private DirectoryInfo DetermineSaveDirectory(FileInfo source)
        {
            if (string.IsNullOrWhiteSpace(Options.CssGenerationOutputDirectory))
                return source.Directory;

            var path = new Stack<string>();
            var current = source.Directory;
            while (current != null && ContainsSassFiles(current.Parent))
            {
                path.Push(current.Name);
                current = current.Parent;
            }

            // eh, things aren't working out so well, just go back to default
            if (current == null || current.Parent == null)
                return source.Directory;

            // move to sibling directory
            current = new DirectoryInfo(Path.Combine(current.Parent.FullName, Options.CssGenerationOutputDirectory));
            while (path.Count > 0)
                current = new DirectoryInfo(Path.Combine(current.FullName, path.Pop()));

            EnsureDirectory(current);
            return current;
        }

        private void EnsureDirectory(DirectoryInfo current)
        {
            if (current != null && !current.Exists)
            {
                EnsureDirectory(current.Parent);
                current.Create();
            }
        }

        private bool ContainsSassFiles(DirectoryInfo directory)
        {
            return directory != null && directory.EnumerateFiles("*.s?ss").Any(x => x.Extension.EndsWith("sass", StringComparison.InvariantCultureIgnoreCase) || x.Extension.EndsWith("scss", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
