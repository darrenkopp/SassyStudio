using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using NSass;
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
                if (filename.StartsWith("_") || !filename.EndsWith(".scss", StringComparison.OrdinalIgnoreCase))
                    return;

                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => GenerateCss(e.Time, e.FilePath)), DispatcherPriority.Background);
            }
        }

        private void GenerateCss(DateTime time, string path)
        {
            var source = new FileInfo(path);
            // file is stale, likely another request coming in
            if (time < source.LastWriteTime)
                return;

            var filename = Path.GetFileNameWithoutExtension(source.Name);
            var target = new FileInfo(Path.Combine(source.Directory.FullName, filename + ".css"));
            var minifiedTarget = new FileInfo(Path.Combine(source.Directory.FullName, filename + ".min.css"));

            try
            {
                var output = Compiler.CompileFile(path, sourceComments: Options.IncludeSourceComments, additionalIncludePaths: new[] { source.Directory.FullName });
                File.WriteAllText(target.FullName, output, UTF8_ENCODING);

                if (Options.GenerateMinifiedCssOnSave)
                    Minify(output, minifiedTarget);

                if (Options.IncludeCssInProject)
                    AddFileToProject(source, target, Options);
            }
            catch (Exception ex)
            {
                if (Options.ReplaceCssWithException)
                    SaveExceptionToFile(ex, target);

                Logger.Log(ex, "Failed to compile css");
            }
        }

        private void Minify(string css, FileInfo file)
        {
            try
            {
                var compressor = new CssCompressor();
                var minified = compressor.Compress(css);

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
