using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Intellisense
{
    [Export(typeof(ICompletionValueProvider))]
    class FunctionsProvider : ValueProviderBase
    {
        public override IEnumerable<SassCompletionContextType> SupportedContexts
        {
            get
            {
                yield return SassCompletionContextType.ConditionalDirectiveExpression;
                yield return SassCompletionContextType.FunctionArgumentValue;
                yield return SassCompletionContextType.FunctionArgumentDefaultValue;
                yield return SassCompletionContextType.IncludeDirectiveMixinArgumentValue;
                yield return SassCompletionContextType.MixinDirectiveMixinArgumentDefaultValue;
                yield return SassCompletionContextType.StringInterpolationValue;
                yield return SassCompletionContextType.VariableValue;
                yield return SassCompletionContextType.PropertyValue;
            }
        }

        public override IEnumerable<Completion> GetCompletions(SassCompletionContextType type, SassCompletionContext context)
        {
            foreach (var builtInFunction in Scss.ScssWellKnownFunctionNames.Names)
                yield return Function(builtInFunction);

            foreach (var userFunction in context.TraversalPath.SelectMany(x => x.Children.OfType<UserFunctionDefinition>()))
                yield return Function(userFunction.GetName(context.Text), item: StandardGlyphItem.GlyphItemInternal);
        }
    }
}
