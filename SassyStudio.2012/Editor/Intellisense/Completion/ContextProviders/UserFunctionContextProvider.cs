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
        public IEnumerable<SassCompletionContextType> GetContext(ICompletionContext context)
        {
            var current = context.Current;
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
