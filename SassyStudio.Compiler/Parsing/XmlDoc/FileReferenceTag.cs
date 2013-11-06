using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class FileReferenceTag : XmlDocumentationTag
    {
        public ISassDocument Document { get; protected set; }
        public XmlAttribute Filename { get; protected set; }

        protected override void OnAttributeParsed(XmlAttribute attribute, ITextProvider text)
        {
            if (attribute.Name != null && IsPathAttribute(text.GetText(attribute.Name.Start, attribute.Name.Length)))
                Filename = attribute;
        }

        private bool IsPathAttribute(string attributeName)
        {
            switch (attributeName)
            {
                case "file":
                case "path":
                    return true;
                default:
                    return false;
            }
        }

        public void ResolveImports(ITextProvider text, ISassDocument document, IDocumentManager documentManager)
        {
            try
            {
                if (Filename == null || Filename.Value == null) 
                    return;

                var path = ImportResolver.ResolvePath(Filename.Value, text, document.Source.Directory);
                if (string.IsNullOrEmpty(path))
                    return;

                var importFile = new FileInfo(path);
                if (importFile.Exists)
                    Document = documentManager.Import(importFile, document);
            }
            catch (Exception ex)
            {
                OutputLogger.Log(ex, "Failed to process reference file.");
            }
        }
    }
}
