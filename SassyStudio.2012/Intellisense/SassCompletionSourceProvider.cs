using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using SassyStudio.Scss;

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

        [ImportMany(typeof(ICompletionProvider))]
        private IEnumerable<ICompletionProvider> CompletionProviders { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new SassCompletionSource(textBuffer, CompletionProviders, TextNavigator);
        }
    }
}
