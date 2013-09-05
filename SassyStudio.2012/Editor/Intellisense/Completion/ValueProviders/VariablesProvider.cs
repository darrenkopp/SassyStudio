using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionValueProvider))]
    class VariablesProvider : ICompletionValueProvider
    {
        public IEnumerable<SassCompletionContextType> SupportedContexts
        {
            get
            {
                // normal variable references
                yield return SassCompletionContextType.ConditionalDirectiveExpression;
                yield return SassCompletionContextType.EachDirectiveListValue;
                yield return SassCompletionContextType.FunctionBody;
                yield return SassCompletionContextType.PropertyValue;
                yield return SassCompletionContextType.StringInterpolationValue;
                yield return SassCompletionContextType.VariableName;
                yield return SassCompletionContextType.VariableValue;
                yield return SassCompletionContextType.WhileLoopCondition;

                // function arguments
                yield return SassCompletionContextType.FunctionArgumentDefaultValue;
                yield return SassCompletionContextType.IncludeDirectiveMixinArgument;
                yield return SassCompletionContextType.IncludeDirectiveMixinArgumentName;
                yield return SassCompletionContextType.IncludeDirectiveMixinArgumentValue;
                yield return SassCompletionContextType.MixinDirectiveMixinArgumentDefaultValue;
            }
        }

        public IEnumerable<ICompletionValue> GetCompletions(SassCompletionContextType type, ICompletionContext context)
        {
            switch (type)
            {
                // standard variable references
                case SassCompletionContextType.ConditionalDirectiveExpression:
                case SassCompletionContextType.EachDirectiveListValue:
                case SassCompletionContextType.FunctionBody:
                case SassCompletionContextType.PropertyValue:
                case SassCompletionContextType.StringInterpolationValue:
                case SassCompletionContextType.VariableName:
                case SassCompletionContextType.VariableValue:
                case SassCompletionContextType.WhileLoopCondition:
                    return context.Cache.GetVariables(context.Position);
                case SassCompletionContextType.FunctionArgumentDefaultValue:
                case SassCompletionContextType.IncludeDirectiveMixinArgument:
                case SassCompletionContextType.IncludeDirectiveMixinArgumentName:
                case SassCompletionContextType.IncludeDirectiveMixinArgumentValue:
                case SassCompletionContextType.MixinDirectiveMixinArgumentDefaultValue:
                    // TODO: named arguments    
                    break;
            }

            return Enumerable.Empty<ICompletionValue>();
        }
    }
}
