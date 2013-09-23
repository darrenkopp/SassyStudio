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
    class VariableDefinitionContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(ICompletionContext context)
        {
            var current = context.Current;
            var predecessor = context.Predecessor;
            if (IsDefinitionScope(current))
            {
                yield return SassCompletionContextType.VariableName;
            }
            else if (current is VariableDefinition && IsDefinitionScope(current.Parent))
            {
                var definition = current as VariableDefinition;
                if (definition.Name == null)
                {
                    yield return SassCompletionContextType.VariableName;
                }
                else
                {
                    yield return SassCompletionContextType.VariableValue;

                    if (definition.Values.Any(x => x.IsValid))
                        yield return SassCompletionContextType.VariableDefaultFlag;
                }
            }
            else if (IsUnclosedVariable(predecessor))
            {
                yield return SassCompletionContextType.VariableDefaultFlag;
            }
        }

        static bool IsUnclosedVariable(ParseItem current)
        {
            while (current != null)
            {
                var definition = current as VariableDefinition;
                if (definition != null && definition.Semicolon == null)
                    return true;

                current = current.Parent;
            }

            return false;
        }

        static bool IsDefinitionScope(ParseItem current)
        {
            return current is Stylesheet || current is BlockItem;
        }
    }
}
