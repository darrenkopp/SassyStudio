using SassyStudio.Compiler.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Intellisense
{
    [Export(typeof(ICompletionContextProvider))]
    class ExtendContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(SassCompletionContext context)
        {
            if (context.Current is RuleBlock)
            {
                yield return SassCompletionContextType.ExtendDirective;
            }
            else if (context.Current is ExtendDirective)
            {
                var directive = context.Current as ExtendDirective;
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
