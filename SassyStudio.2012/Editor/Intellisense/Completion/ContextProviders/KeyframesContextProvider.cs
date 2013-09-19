using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;
using SassyStudio.Compiler.Parsing.Rules;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionContextProvider))]
    class KeyframesContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(ParseItem current, ParseItem predecessor, int position)
        {
            if (current is Stylesheet || current is MediaQueryBlock)
                yield return SassCompletionContextType.KeyframesDirective;

            if (current is KeyframeRuleBlock)
                yield return SassCompletionContextType.KeyframesNamedRange;
        }
    }
}
