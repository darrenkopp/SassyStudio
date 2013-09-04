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
    class PropertyValueContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(ParseItem current, int position)
        {
            if (current is RuleBlock)
            {
                yield return SassCompletionContextType.PropertyDeclaration;
            }
            else if (current is PropertyDeclaration)
            {
                yield return SassCompletionContextType.PropertyValue;
            }
        }
    }
}
