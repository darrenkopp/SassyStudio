﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using NSass;
using SassyStudio.Compiler.Parsing;
using SassyStudio.Options;
using Yahoo.Yui.Compressor;

namespace SassyStudio.Editor
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class GenerateCssOnSave : IWpfTextViewCreationListener
    {
        static int IsResolverInitialized = 0;
        static readonly Encoding UTF8_ENCODING = new UTF8Encoding(true);
        readonly Lazy<ISassCompiler> _Compiler = new Lazy<ISassCompiler>(() => new SassCompiler());
        readonly Lazy<ScssOptions> _Options = new Lazy<ScssOptions>(() => SassyStudioPackage.Instance.Options.Scss, true);

        private ScssOptions Options { get { return _Options.Value; } }
        private ISassCompiler Compiler { get { return _Compiler.Value; } }

        readonly IRootLevelDocumentCache DocumentCache;

        [ImportingConstructor]
        public GenerateCssOnSave(IRootLevelDocumentCache documentCache)
        {
            DocumentCache = documentCache;
        }

        public void TextViewCreated(IWpfTextView textView)
        {
            ITextDocument document;
            if (textView.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document))
            {
                FixNSassAssemblyResolution();
                document.FileActionOccurred += OnFileActionOccurred;
            }
        }

        private void OnFileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
        {
            if (e.FileActionType == FileActionTypes.ContentSavedToDisk)
            {
                if (!Options.GenerateCssOnSave) return;

                var filename = Path.GetFileName(e.FilePath);

                // ignore anything that isn't .scss and not a root document
                if (!filename.EndsWith(".scss", StringComparison.OrdinalIgnoreCase))
                    return;

                if (filename.StartsWith("_"))
                {
                    Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => GenerateAllReferencing(e.Time, e.FilePath)), DispatcherPriority.Background);
                }
                else
                {
                    Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => GenerateCss(e.Time, e.FilePath)), DispatcherPriority.Background);
                }
            }
        }

        private void GenerateAllReferencing(DateTime time, string path)
        {
            var source = new FileInfo(path);
            var documents = DocumentCache.Documents;

            var visited = new HashSet<ISassDocument>();
            foreach (var document in documents)
            {
                if (IsReferenced(source, document, visited))
                    GenerateCss(time, document.Source.FullName);
            }
        }

        private bool IsReferenced(FileInfo source, ISassDocument document, HashSet<ISassDocument> visited)
        {
            var comparer = StringComparer.CurrentCultureIgnoreCase;
            foreach (var import in document.Stylesheet.Children.OfType<SassImportDirective>().SelectMany(x => x.Files).Where(x => x.Document != null))
            {
                if (!visited.Add(import.Document)) 
                    continue;

                if (comparer.Equals(import.Document.Source.FullName, source.FullName) || IsReferenced(source, import.Document, visited))
                    return true;
            }

            return false;
        }

        private void GenerateCss(DateTime time, string path)
        {
            var source = new FileInfo(path);
            // file is stale, likely another request coming in
            if (time < source.LastWriteTime)
                return;

            var filename = Path.GetFileNameWithoutExtension(source.Name);
            var directory = DetermineSaveDirectory(source);
            var target = new FileInfo(Path.Combine(directory.FullName, filename + ".css"));
            var minifiedTarget = new FileInfo(Path.Combine(directory.FullName, filename + ".min.css"));

            IEnumerable<string> includePaths = new[] { source.Directory.FullName };
            if (!string.IsNullOrWhiteSpace(Options.CompilationIncludePaths) && Directory.Exists(Options.CompilationIncludePaths))
                includePaths = includePaths.Concat(Options.CompilationIncludePaths.Split(new[] {';' }, StringSplitOptions.RemoveEmptyEntries));

            try
            {
                var output = Compiler.CompileFile(path, sourceComments: Options.IncludeSourceComments, additionalIncludePaths: includePaths);
                File.WriteAllText(target.FullName, output, UTF8_ENCODING);

                if (Options.GenerateMinifiedCssOnSave)
                    Minify(output, minifiedTarget);

                // only add to project if options allow it and not moving to another directory
                if (Options.IncludeCssInProject && string.IsNullOrWhiteSpace(Options.CssGenerationOutputDirectory))
                    AddFileToProject(source, target, Options);
            }
            catch (Exception ex)
            {
                if (Options.ReplaceCssWithException)
                    SaveExceptionToFile(ex, target);

                Logger.Log(ex, "Failed to compile css");
            }
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
            return directory != null && directory.EnumerateFiles("*.scss").Any();
        }

        private void Minify(string css, FileInfo file)
        {
            try
            {
                string minified = "";
                if (!string.IsNullOrEmpty(css))
                {
                    var compressor = new CssCompressor { RemoveComments = true };
                    minified = compressor.Compress(css);
                }

                File.WriteAllText(file.FullName, minified, UTF8_ENCODING);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Failed to generate minified css file.");
                if (Options.ReplaceCssWithException)
                    SaveExceptionToFile(ex, file);
            }
        }

        private void SaveExceptionToFile(Exception error, FileInfo target)
        {
            try
            {
                File.WriteAllText(target.FullName,
                    new StringBuilder()
                        .AppendLine("/*")
                        .AppendLine(error.Message)
                        .AppendLine(error.StackTrace)
                        .AppendLine("*/")
                    .ToString(),
                    UTF8_ENCODING
                );
            }
            catch
            {
                // ignore
            }
        }

        private static void AddFileToProject(FileInfo source, FileInfo target, ScssOptions options)
        {
            var buildAction = options.IncludeCssInProjectOutput ? InteropHelper.BuildActionType.Content : InteropHelper.BuildActionType.None;
            InteropHelper.AddNestedFile(SassyStudioPackage.Instance.DTE, source.FullName, target.FullName, buildAction);
        }

        private static void FixNSassAssemblyResolution()
        {
            if (Interlocked.CompareExchange(ref IsResolverInitialized, 1, 0) == 0)
            {
                var basePath = new FileInfo(new Uri(typeof(GenerateCssOnSave).Assembly.CodeBase).LocalPath).Directory.FullName;
                AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
                {
                    if (e.Name.StartsWith("NSass.Wrapper.proxy", StringComparison.Ordinal))
                        return Assembly.LoadFrom(Path.Combine(basePath, "NSass.Wrapper.x86.dll"));

                    return null;
                };
            }
        }
    }
}
