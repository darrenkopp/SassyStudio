using System.Collections.Generic;
using System.ComponentModel.Composition;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    //[Export(typeof(ICompletionContextProvider))]
    class SelectorContextProvider : ICompletionContextProvider
    {
        readonly ICssSchemaManager SchemaManager;

        [ImportingConstructor]
        public SelectorContextProvider(ICssSchemaManager schemaManager)
        {
            SchemaManager = schemaManager;
        }

        public IEnumerable<SassCompletionContextType> GetContext(ICompletionContext context)
        {
            if (IsValidProperty(context))
                yield break;

            if (FindSelector(context.Current) != null || IsNestedSelector(context.Current))
            {
                yield return SassCompletionContextType.PseudoClass;
                yield return SassCompletionContextType.PseudoElement;
                yield return SassCompletionContextType.PseudoFunction;
            }
            else if (FindSelector(context.Predecessor) != null && IsNestedSelector(context.Predecessor))
            {
                yield return SassCompletionContextType.PseudoClass;
                yield return SassCompletionContextType.PseudoElement;
                yield return SassCompletionContextType.PseudoFunction;
            }
        }

        private bool IsValidProperty(ICompletionContext context)
        {
            var schema = SchemaManager.CurrentSchema;
            if (schema == null)
                return false;

            return schema.IsProperty(context.DocumentTextProvider.GetText(context.Current.Start, context.Current.Length));
        }

        private SimpleSelector FindSelector(ParseItem item)
        {
            while (item != null)
            {
                if (item is SimpleSelector)
                    return item as SimpleSelector;

                item = item.Parent;
            }

            return null;
        }

        private bool IsNestedSelector(ParseItem current)
        {
            while (current != null)
            {
                if (current is RuleBlock)
                    return true;

                current = current.Parent;
            }

            return false;
        }
    }
}
