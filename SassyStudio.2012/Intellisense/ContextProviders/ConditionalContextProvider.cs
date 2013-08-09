using System.Collections.Generic;
using System.ComponentModel.Composition;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Intellisense
{
    [Export(typeof(ICompletionContextProvider))]
    class ConditionalContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(SassCompletionContext context)
        {
            if (context.Current is Stylesheet || context.Current is BlockItem)
            {
                yield return SassCompletionContextType.ConditionalDirective;
            }
            else if (context.Current is ConditionalControlDirective)
            {
                var directive = context.Current as ConditionalControlDirective;
                if (directive.Rule != null && directive.Rule.Name != null && AllowsExpresion(context.Text, directive.Rule.Name))
                    yield return SassCompletionContextType.ConditionalDirectiveExpression;
            }
        }

        static bool AllowsExpresion(ITextProvider text, TokenItem statement)
        {
            var name = text.GetText(statement.Start, statement.Length);

            return name == "if" || name == "else if";
        }
    }
}
