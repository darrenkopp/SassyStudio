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
            var position = context.Position;
            if (current is PropertyDeclaration)
            {
                yield return SassCompletionContextType.PropertyValue;
            }
            else if (IsInDeclarationContext(current))
            {
                yield return SassCompletionContextType.PropertyDeclaration;
            }
            else
            {
                var declaration = FindDeclaration(current);
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

        static bool IsInDeclarationContext(ParseItem current)
        {
            // exclude when not nested in rule block
            if (current is ControlDirectiveBody && (current.Parent == null || !(current.Parent.Parent is RuleBlock)))
                return false;

            return current is RuleBlock || current is NestedPropertyBlock;
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
