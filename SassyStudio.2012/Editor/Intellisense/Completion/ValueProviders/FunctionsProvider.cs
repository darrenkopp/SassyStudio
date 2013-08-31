using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using SassyStudio.Compiler.Parsing;
using SassyStudio.Scss;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionValueProvider))]
    class FunctionsProvider : ICompletionValueProvider
    {
        static readonly IEnumerable<ICompletionValue> SystemFunctions = ScssWellKnownFunctionNames.Names.Select(name => new SystemFunctionCompletionValue(name)).ToArray();

        public IEnumerable<SassCompletionContextType> SupportedContexts
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

        public IEnumerable<ICompletionValue> GetCompletions(SassCompletionContextType type, ICompletionContext context)
        {
            return SystemFunctions.Concat(context.Cache.GetFunctions(context.Position));
        }
    }
}
