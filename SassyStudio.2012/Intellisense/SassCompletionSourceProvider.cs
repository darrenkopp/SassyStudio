using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace SassyStudio.Intellisense
{
    [Export(typeof(ICompletionSourceProvider))]
    [Name("SCSS Completion Source Provider"), Order(Before = "Default")]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    class SassCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        internal ITextStructureNavigatorSelectorService TextNavigator { get; set; }

        [ImportMany(typeof(ISassCompletionAugmenter))]
        private IEnumerable<ISassCompletionAugmenter> CompletionAugmenters { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new SassCompletionSource(textBuffer, CompletionAugmenters, TextNavigator);
        }
    }
}
