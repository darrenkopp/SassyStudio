using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Intellisense
{
    [Export(typeof(ICompletionContextProvider))]
    class UserFunctionContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(SassCompletionContext context)
        {
            if (context.Current is Stylesheet)
            {
                yield return SassCompletionContextType.FunctionDirective;
            }
            else if (context.Current is UserFunctionBody)
            {
                yield return SassCompletionContextType.FunctionBody;
            }
        }
    }
}
