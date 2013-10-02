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
    class PropertyValueContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(ICompletionContext context)
        {
            var current = context.Current;
            var predecessor = context.Predecessor;
            var position = context.Position;
            if (current is PropertyDeclaration)
            {
                yield return SassCompletionContextType.PropertyValue;
            }
            else if (current is RuleBlock)
            {
                yield return SassCompletionContextType.PropertyDeclaration;
            }
            else
            {
                var declaration = FindDeclaration(current) ?? FindDeclaration(predecessor);
                if (declaration == null)
                    yield break;

                if (declaration.Colon == null || declaration.Colon.Start > position)
                {
                    yield return SassCompletionContextType.PropertyName;
                }
                else if (declaration.Colon != null && (declaration.End > position || declaration.IsUnclosed))
                {
                    yield return SassCompletionContextType.PropertyValue;
                }
            }
        }

        static PropertyDeclaration FindDeclaration(ParseItem item)
        {
            while (item != null)
            {
                if (item is PropertyDeclaration)
                    return item as PropertyDeclaration;

                item = item.Parent;
            }

            return null;
        }
    }
}
