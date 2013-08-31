using System.Collections.Generic;
using System.ComponentModel.Composition;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionContextProvider))]
    class ConditionalContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(ParseItem current, int position)
        {
            if (current is Stylesheet || current is BlockItem)
            {
                yield return SassCompletionContextType.ConditionalDirective;
            }
            else if (current is ConditionalControlDirective)
            {
                var directive = current as ConditionalControlDirective;
                if (directive.Rule != null && directive.Rule.Name != null /*&& AllowsExpresion(context.Text, directive.Rule.Name)*/)
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
