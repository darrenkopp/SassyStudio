using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Shell;
using SassyStudio.Compiler;
using SassyStudio.Compiler.Parsing;
using Task = System.Threading.Tasks.Task;

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
            var configDirectory = ResolveProject(source.Directory);

            if (configDirectory != null)
            {
                var configRbFilePath = configDirectory.FullName + "\\config.rb";

                var outputDir = (from line in File.ReadAllLines(configRbFilePath)
                    let record = ParseCompassConfigLine(line)
                    where record.Key == "css_dir"
                    select record.Value).FirstOrDefault() ?? "stylesheets"; //"stylesheets" is default as per: http://compass-style.org/help/tutorials/configuration-reference/

                outputDir = outputDir.Trim(new[] { '"', '\'' });
                var outputFileName = Path.GetFileNameWithoutExtension(source.FullName) + ".css";
                var outputFileInfo = new FileInfo(configDirectory.FullName + "\\" + outputDir + "\\" + outputFileName);
                return outputFileInfo;
            }

            return null;
        }

        private static KeyValuePair<string, string> ParseCompassConfigLine(string line)
        {
            line = line.Split('#').First(); //strip comments

            var keyValueArray = line.Split(new[] { '=' }, 2); //split on first '='

            if (keyValueArray.Length == 2) //if key/value assignment found then process and return
            {
                return new KeyValuePair<string, string>(keyValueArray[0].Trim(), keyValueArray[1].Trim());
            }

            return new KeyValuePair<string, string>("", "");
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
