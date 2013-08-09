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
    class VariableDefinitionContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(SassCompletionContext context)
        {
            if (IsDefinitionScope(context.Current))
            {
                yield return SassCompletionContextType.VariableName;
            }
            else if (context.Current is VariableDefinition && IsDefinitionScope(context.Current.Parent))
            {
                var definition = context.Current as VariableDefinition;
                if (definition.Name == null)
                {
                    yield return SassCompletionContextType.VariableName;
                }
                else
                {
                    yield return SassCompletionContextType.VariableValue;

                    if (definition.Values.Where(x => x.IsValid).Any())
                        yield return SassCompletionContextType.VariableDefaultFlag;
                }
            }
        }

        static bool IsDefinitionScope(ParseItem current)
        {
            return current is Stylesheet || current is BlockItem;
        }
    }
}
