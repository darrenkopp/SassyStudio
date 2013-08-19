using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

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

        [ImportingConstructor]
        public SassEditorManager(IDocumentManager documentManager, IForegroundParsingTask parsingTask, IBackgroundParsingTask backgroundParsingTask)
        {
            DocumentManager = documentManager;
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
                return new SassEditor(buffer, DocumentManager.Get(new FileInfo(document.FilePath)), ParsingTask);

            return null;
        }
    }
}
