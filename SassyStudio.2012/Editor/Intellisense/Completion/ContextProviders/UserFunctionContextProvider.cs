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
    class UserFunctionContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(ParseItem current, int position)
        {
            if (current is Stylesheet)
            {
                yield return SassCompletionContextType.FunctionDirective;
            }
            else if (current is UserFunctionBody)
            {
                yield return SassCompletionContextType.FunctionBody;
            }
        }
    }
}
