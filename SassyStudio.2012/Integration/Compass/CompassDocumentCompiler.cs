using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler;

namespace SassyStudio.Integration.Compass
{
    class CompassDocumentCompiler : IDocumentCompiler
    {
        public Task CompileAsync(FileInfo source, FileInfo output)
        {
            return Task.Run(() => Compile(source, output));
        }

        public void Compile(FileInfo source, FileInfo output)
        {
            var project = ResolveProject(source.Directory);
            if (project == null)
                throw new InvalidOperationException(string.Format("Cannot compile '{0}' with compass because we are not in a compass project.", source.Name));

            var start = new ProcessStartInfo
            {
                FileName = CompassSupport.CompassBatchFile,
                Arguments = GetCompassArguments(source, project),
                WorkingDirectory = project.FullName,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            using (var executor = Process.Start(start))
            {
                StringBuilder standardOutput = new StringBuilder(), errorOutput = new StringBuilder();
                executor.OutputDataReceived += (sender, e) => standardOutput.AppendLine(e.Data);
                executor.ErrorDataReceived += (sender, e) => errorOutput.AppendLine(e.Data);
                executor.BeginOutputReadLine();
                executor.BeginErrorReadLine();

                executor.WaitForExit();

                if (executor.ExitCode != 0)
                {
                    var message = "Compass returned an error.";
                    if (errorOutput.Length > 0)
                    {
                        message += Environment.NewLine + errorOutput.ToString();
                    }
                    else if (standardOutput.Length > 0)
                    {
                        message += Environment.NewLine + standardOutput.ToString();
                    }

                    throw new Exception(message);
                }
            }
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
