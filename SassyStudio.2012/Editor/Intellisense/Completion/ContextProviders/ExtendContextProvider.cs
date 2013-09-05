using SassyStudio.Compiler.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionContextProvider))]
    class ExtendContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(ParseItem current, int position)
        {
            if (current is RuleBlock)
            {
                yield return SassCompletionContextType.ExtendDirective;
            }
            else if (current is ExtendDirective)
            {
                var directive = current as ExtendDirective;
                if (directive.Selector == null)
                {
                    yield return SassCompletionContextType.ExtendDirectiveReference;
                }
                else if (directive.Selector != null && directive.Modifier == null)
                {
                    yield return SassCompletionContextType.ExtendDirectiveOptionalFlag;
                }
            }
        }
    }
}
