using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using SassyStudio.Editor.Intellisense;

namespace SassyStudio.Editor
{
    public interface ISassEditorManager
    {
        ISassEditor Get(ITextBuffer buffer);
    }

    [Export(typeof(ISassEditorManager))]
    [ContentType("SCSS")]
    public class SassEditorManager : ISassEditorManager
    {
        readonly IForegroundParsingTask ParsingTask;
        readonly IDocumentManager DocumentManager;
        readonly IIntellisenseManager IntellisenseManager;

        [ImportingConstructor]
        public SassEditorManager(IDocumentManager documentManager, IIntellisenseManager intellisenseManager, IForegroundParsingTask parsingTask, IBackgroundParsingTask backgroundParsingTask)
        {
            DocumentManager = documentManager;
            IntellisenseManager = intellisenseManager;
            ParsingTask = parsingTask;

            // start background monitoring
            backgroundParsingTask.Start();
        }

        public ISassEditor Get(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => Create(buffer));
        }

        ISassEditor Create(ITextBuffer buffer)
        {
            ITextDocument document;
            if (buffer.Properties.TryGetProperty(typeof(ITextDocument), out document))
            {
                var sassDocument = DocumentManager.Get(new FileInfo(document.FilePath));
                var cache = IntellisenseManager.Get(sassDocument);
                return new SassEditor(buffer, sassDocument, cache, ParsingTask);
            }

            return null;
        }
    }
}
