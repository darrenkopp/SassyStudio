using System.Collections.Generic;
using System.ComponentModel.Composition;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionContextProvider))]
    class SelectorContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(ParseItem current, ParseItem predecessor, int position)
        {
            if (current is RuleBlock || current is SelectorGroup || current.Parent is SelectorGroup)
            {
                //var predecessor = current.PreviousSibling();
                //if (predecessor is SimpleSelector)
                //{
                    yield return SassCompletionContextType.PseudoClass;
                    yield return SassCompletionContextType.PseudoElement;
                    yield return SassCompletionContextType.PseudoFunction;
                //}
            }
        }
    }
}
