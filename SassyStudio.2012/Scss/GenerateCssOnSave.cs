using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using NSass;

namespace SassyStudio.Scss
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class GenerateCssOnSave : IWpfTextViewCreationListener
    {
        ISassCompiler Compiler;

        public void TextViewCreated(IWpfTextView textView)
        {
            ITextDocument document;
            if (textView.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document))
            {
                FixNSassAssemblyResolution();
                document.FileActionOccurred += OnFileActionOccurred;
                Compiler = new SassCompiler();
            }
        }

        private void OnFileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
        {
            if (e.FileActionType == FileActionTypes.ContentSavedToDisk)
            {
                var source = new FileInfo(e.FilePath);
                var target = Path.Combine(source.Directory.FullName, Path.GetFileNameWithoutExtension(source.Name) + ".css");

                try
                {
                    var output = Compiler.CompileFile(source.FullName, sourceComments: false, additionalIncludePaths: new[] { source.Directory.FullName });
                    File.WriteAllText(target, output);
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void FixNSassAssemblyResolution()
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
