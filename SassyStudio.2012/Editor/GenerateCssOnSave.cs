using System;
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
using SassyStudio.Compiler;
using SassyStudio.Compiler.Parsing;
using SassyStudio.Integration.Compass;
using SassyStudio.Integration.LibSass;
using SassyStudio.Integration.SassGem;
using SassyStudio.Options;
using Yahoo.Yui.Compressor;

namespace SassyStudio.Editor
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class GenerateCssOnSave : IWpfTextViewCreationListener
    {
        static readonly Encoding UTF8_ENCODING = new UTF8Encoding(true);
        readonly Lazy<ScssOptions> _Options = new Lazy<ScssOptions>(() => SassyStudioPackage.Instance.Options.Scss, true);

        private ScssOptions Options { get { return _Options.Value; } }

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
                document.FileActionOccurred += OnFileActionOccurred;
            }
            else
            {
                if (Options.IsDebugLoggingEnabled)
                    Logger.Log("Eh? Couldn't find text document. Can't handle saving documents now.");
            }
        }

        private void OnFileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
        {
            if (e.FileActionType == FileActionTypes.ContentSavedToDisk)
            {
                if (Options.IsDebugLoggingEnabled)
                    Logger.Log("Detected file saved: " + e.FilePath);

                if (!Options.GenerateCssOnSave) return;

                var filename = Path.GetFileName(e.FilePath);

                // ignore anything that isn't .scss and not a root document
                if (!filename.EndsWith(".scss", StringComparison.OrdinalIgnoreCase))
                    return;

                if (filename.StartsWith("_"))
                {
                    if (Options.IsDebugLoggingEnabled)
                        Logger.Log("Compiling all files referencing include file: " + filename);

                    Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => GenerateAllReferencing(e.Time, e.FilePath)), DispatcherPriority.Background);
                }
                else
                {
                    if (Options.IsDebugLoggingEnabled)
                        Logger.Log("Compiling: " + filename);

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
                if (comparer.Equals(import.Document.Source.FullName, source.FullName))
                    return true;

                if (!visited.Add(import.Document))
                    continue;

                if (IsReferenced(source, import.Document, visited))
                    return true;
            }

            return false;
        }

        private void GenerateCss(DateTime time, string path)
        {
            if (Options.IsDebugLoggingEnabled)
                Logger.Log("Beginning compile: " + path);

            var source = new FileInfo(path);
            // file is stale, likely another request coming in
            if (time < source.LastWriteTime)
            {
                if (Options.IsDebugLoggingEnabled)
                    Logger.Log("Ignoring compile due to stale document.");

                return;
            }

            var filename = Path.GetFileNameWithoutExtension(source.Name);
            var document = new FileInfo(path);
            var compiler = PickCompiler(document);
            var output = compiler.GetOutput(document);

            try
            {
                compiler.Compile(document, output);

                // minify
                if (Options.GenerateMinifiedCssOnSave && output != null)
                    Minify(File.ReadAllText(output.FullName), new FileInfo(Path.Combine(output.Directory.FullName, filename + ".min.css")));

                // add to project
                if (Options.IncludeCssInProject && output != null && string.IsNullOrWhiteSpace(Options.CssGenerationOutputDirectory))
                    AddFileToProject(source, output, Options);
            }
            catch (Exception ex)
            {
                if (Options.ReplaceCssWithException && output != null)
                    SaveExceptionToFile(ex, output);

                Logger.Log(ex, "Failed to compile css.");
            }

            if (Options.IsDebugLoggingEnabled)
                Logger.Log("Compile complete.");
        }

        private IDocumentCompiler PickCompiler(FileInfo document)
        {
            if (CompassSupport.IsCompassInstalled && CompassSupport.IsInCompassProject(document.Directory))
                return new CompassDocumentCompiler();

            if (SassSupport.IsSassGemInstalled)
                return new SassDocumentCompiler(Options);

            return new NSassDocumentCompiler(Options);
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
    }
}
