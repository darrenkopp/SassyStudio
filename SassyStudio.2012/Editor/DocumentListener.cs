using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using SassyStudio.Editor.Intellisense;

namespace SassyStudio.Editor
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class DocumentListener : IVsTextViewCreationListener
    {
        [Import, System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        internal ICompletionBroker CompletionBroker { get; set; }

        [Import]
        IIntellisenseManager IntellisenseManager { get; set; }

        [Import]
        ISassEditorManager EditorManager { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            var textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);

            textView.Properties.GetOrCreateSingletonProperty(() => new CommentSelectionCommandHandler(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new FormatDocumentHandler(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new GoToDefinitionCommandHandler(textViewAdapter, textView, EditorManager));

            if (SassyStudioPackage.Instance.Options.Scss.EnableExperimentalIntellisense)
                textView.Properties.GetOrCreateSingletonProperty(() => new CompletionCommandHandler(CompletionBroker, textViewAdapter, textView));
        }
    }
}
