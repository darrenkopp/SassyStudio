using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class SassImportDirective : ImportDirective
    {
        readonly List<ImportFile> _Files = new List<ImportFile>(0);

        public IList<ImportFile> Files { get { return _Files; } }

        protected override void ParseImport(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            while (!IsTerminator(stream.Current.Type))
            {
                var file = itemFactory.CreateSpecific<ImportFile>(this, text, stream) ?? new ImportFile();
                if (!file.Parse(itemFactory, text, stream))
                    break;

                Children.Add(file);
                Files.Add(file);
            }
        }

        static bool IsTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.Semicolon:
                    return true;
            }

            return false;
        }

        public override void Freeze()
        {
            base.Freeze();

            if (_Files.Count > 0)
                _Files.TrimExcess();
        }

        public void ResolveImports(ITextProvider text, ISassDocument document, IDocumentManager documentManager)
        {
            foreach (var file in Files)
            {
                try
                {
                    var path = file.ResolvePath(document.Source.Directory, text);
                    if (string.IsNullOrEmpty(path))
                        continue;

                    var importFile = new FileInfo(path);
                    if (importFile.Exists)
                        file.Document = documentManager.Import(importFile, document);
                }
                catch (Exception ex)
                {
                    OutputLogger.Log(ex, "Failed to process import file.");
                }
            }
        }

        private FileInfo CheckAllPossiblePaths(string path)
        {
            if (!path.EndsWith(".scss"))
                path += ".scss";

            var file = new FileInfo(path);
            if (file.Exists)
                return file;

            // check for include only file
            file = new FileInfo(Path.Combine(file.Directory.FullName, "_" + Path.GetFileName(file.FullName)));
            if (file.Exists)
                return file;

            return new FileInfo(path);
        }
    }
}
