using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing.Selectors;

namespace SassyStudio.Compiler.Parsing
{
    public class PageDirective : AtRuleDirective
    {
        protected override string RuleName { get { return "page"; } }

        public PseudoClassSelector Selector { get; protected set; }

        protected override void ParseDirective(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Semicolon && stream.Peek(1).Type == TokenType.Identifier)
            {
                var selector = itemFactory.CreateSpecific<PseudoClassSelector>(this, text, stream);
                if (selector.Parse(itemFactory, text, stream))
                {
                    Selector = selector;
                    Children.Add(selector);
                }
            }
        }
    }
}
