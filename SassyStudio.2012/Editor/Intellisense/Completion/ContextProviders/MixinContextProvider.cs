using System.Collections.Generic;
using System.ComponentModel.Composition;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionContextProvider))]
    class MixinContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(ParseItem current, int position)
        {
            if (current == null)
                yield break;

            if (current is Stylesheet)
            {
                yield return SassCompletionContextType.MixinDirective;
            }
            else if (current is RuleBlock)
            {
                yield return SassCompletionContextType.IncludeDirective;
            }
            else if (current is MixinReference)
            {
                var reference = current as MixinReference;
                if (reference.Name == null)
                {
                    yield return SassCompletionContextType.IncludeDirectiveMixinName;
                }
                else if (reference.OpenBrace != null)
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
            else if (current is MixinDefinitionBody)
            {
                yield return SassCompletionContextType.MixinBody;
            }
        }
    }
}
