using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using SassyStudio.Compiler;

namespace SassyStudio.Integration.Compass
{
    class CompassDocumentCompiler : IDocumentCompiler
    {
        public void Compile(FileInfo source, FileInfo output)
        {
            var project = ResolveProject(source.Directory);
            if (project == null)
                throw new InvalidOperationException(string.Format("Cannot compile '{0}' with compass because we are not in a compass project.", source.Name));

            var executor = Process.Start(new ProcessStartInfo
            {
                FileName = CompassSupport.CompassBatchFile,
                Arguments = GetCompassArguments(source, project),
                WorkingDirectory = project.FullName,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
            executor.WaitForExit();

            if (executor.ExitCode != 0)
                throw new Exception("Compass returned an error.");
        }

        public FileInfo GetOutput(FileInfo source)
        {
            return null;
        }

        private string GetCompassArguments(FileInfo document, DirectoryInfo project)
        {
            return new StringBuilder()
                .Append("compile").Append(" ")
                //.Append("\"").Append(project.FullName).Append("\"").Append(" ")
                //.Append("\"").Append(document.FullName).Append("\"").Append(" ")
                .Append("--quiet")
            .ToString();
        }

        private DirectoryInfo ResolveProject(DirectoryInfo directory)
        {
            if (directory == null || directory.Root.Equals(directory))
                return null;

            if (directory.EnumerateFiles("config.rb").Any())
                return directory;

            return ResolveProject(directory.Parent);
        }
    }
}
