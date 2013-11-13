using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionContextProvider))]
    class ImportingContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(ICompletionContext context)
        {
            var current = context.Current;
            if (current is Stylesheet || (current is RuleBlock || current.Parent is Stylesheet))
            {
                yield return SassCompletionContextType.ImportDirective;
            }
            else if (current is StringValue && current.Parent is ImportFile)
            {
                yield return SassCompletionContextType.ImportDirectiveFile;
            }
        }
    }
}
