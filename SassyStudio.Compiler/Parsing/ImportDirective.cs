using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class ImportDirective : ComplexItem
    {
        readonly List<ImportFile> _Files = new List<ImportFile>(0);

        public AtRule Rule { get; protected set; }
        public TokenItem Semicolon { get; protected set; }
        public IList<ImportFile> Files { get { return _Files; } }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, "import"))
            {
                Rule = AtRule.CreateParsed(itemFactory, text, stream);
                if (Rule != null)
                    Children.Add(Rule);

                while (!IsTerminator(stream.Current.Type))
                {
                    var file = itemFactory.CreateSpecific<ImportFile>(this, text, stream) ?? new ImportFile();
                    if (!file.Parse(itemFactory, text, stream))
                        break;

                    Children.Add(file);
                    Files.Add(file);
                }

                if (stream.Current.Type == TokenType.Semicolon)
                    Semicolon = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);
            }

            return Children.Count > 0;
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
            var basePath = document.Source.Directory.FullName;
            foreach (var file in Files)
            {
                try
                {
                    var relativePath = text.GetText(file.Path.Start, file.Path.Length);
                    if (relativePath != null && !relativePath.StartsWith("url"))
                        relativePath = relativePath.Trim('"');

                    var fullPath = basePath;
                    var segments = relativePath.Split('/', '\\');
                    foreach (var segment in segments)
                        fullPath = Path.Combine(fullPath, segment);

                    var importFile = CheckAllPossiblePaths(fullPath);
                    if (importFile.Exists && (importFile.Extension.EndsWith("scss") || importFile.Extension.EndsWith("sass")))
                        file.Document = documentManager.Import(importFile, document);
                }
                catch
                {
                    // swallow bad paths
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
