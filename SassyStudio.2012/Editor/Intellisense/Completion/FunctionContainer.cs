using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    class FunctionContainer : CompletionContainerBase
    {
        readonly UserFunctionDefinition Definition;

        public FunctionContainer(UserFunctionDefinition definition)
        {
            Definition = definition;
        }
    }
}
