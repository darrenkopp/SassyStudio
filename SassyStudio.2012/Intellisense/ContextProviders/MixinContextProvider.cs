using System.Collections.Generic;
using System.ComponentModel.Composition;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Intellisense
{
    [Export(typeof(ICompletionContextProvider))]
    class MixinContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(SassCompletionContext context)
        {
            var current = context.Current;
            if (current == null)
                yield break;

            if (current is Stylesheet)
            {
                yield return SassCompletionContextType.MixinDirective;
            }
            else if (current is MixinReference)
            {
                var reference = current as MixinReference;
                if (reference != null && reference.Name == null)
                {
                    yield return SassCompletionContextType.IncludeDirectiveMixinName;
                }
                else if (reference != null && reference.OpenBrace != null)
                {
                    yield return SassCompletionContextType.IncludeDirectiveMixinArgument;
                }
            }
            else if (current is FunctionArgument && current.Parent is MixinReference)
            {                   
                // if current is named argument, then variable has already been named, so we only care about values
                var namedArgument = current as NamedFunctionArgument;
                if (namedArgument == null || namedArgument.Variable == null)
                    yield return SassCompletionContextType.IncludeDirectiveMixinArgumentName;

                yield return SassCompletionContextType.IncludeDirectiveMixinArgumentValue;
            }
        }
    }
}
