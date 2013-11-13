using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    //[Export(typeof(ICompletionContextProvider))]
    class StringValueProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(ICompletionContext context)
        {
            // TODO: figure out when we are in import
            if (context.Current is StringValue)
                yield break;
        }
    }
}
