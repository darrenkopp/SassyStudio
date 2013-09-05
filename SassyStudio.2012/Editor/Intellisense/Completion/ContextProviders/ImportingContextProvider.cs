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
        public IEnumerable<SassCompletionContextType> GetContext(ParseItem current, int position)
        {
            if (current is Stylesheet || (current is RuleBlock || current.Parent is Stylesheet))
                yield return SassCompletionContextType.ImportDirective;
        }
    }
}
