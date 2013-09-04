using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    class MixinContainer : CompletionContainerBase
    {
        readonly MixinDefinition Definition;

        public MixinContainer(MixinDefinition definition)
        {
            Definition = definition;
            Start = Math.Max(definition.Start, ((definition.Body != null && definition.Body.OpenCurlyBrace != null) ? definition.Body.OpenCurlyBrace.Start : 0));
            End = definition.End;
        }
    }
}
