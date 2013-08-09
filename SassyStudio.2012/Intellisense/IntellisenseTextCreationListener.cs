using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace SassyStudio.Intellisense
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class IntellisenseTextCreationListener : IVsTextViewCreationListener
    {
        [Import, System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        internal ICompletionBroker CompletionBroker { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            var view = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            if (view != null)
                view.Properties.GetOrCreateSingletonProperty(() => new CompletionCommandHandler(textViewAdapter, view, CompletionBroker));
        }
    }
}
