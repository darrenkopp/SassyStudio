using SassyStudio.Compiler.Parsing.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public abstract class SimpleSelector : ComplexItem
    {
        public SelectorCombinator Combinator { get; protected set; }

        protected abstract bool ParseSelectorToken(IItemFactory itemFactory, ITextProvider text, ITokenStream stream);

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (ParseSelectorToken(itemFactory, text, stream))
                ParseCombinator(itemFactory, text, stream);

            return Children.Count > 0;
        }

        protected virtual bool ParseCombinator(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            SelectorCombinator combinator = null;
            switch (stream.Current.Type)
            {
                case TokenType.GreaterThan:
                    combinator = new ChildCombinator();
                    break;
                case TokenType.Plus:
                    combinator = new AdjacentSiblingCombinator();
                    break;
                case TokenType.Tilde:
                    combinator = new GeneralSiblingCombinator();
                    break;
            }

            if (combinator != null)
            {
                if (combinator.Parse(itemFactory, text, stream))
                {
                    Children.Add(combinator);
                    Combinator = combinator;
                }
            }
            else if (stream.Current.Type != TokenType.OpenCurlyBrace)
            {
                // whitespace only combinator means no adding to children or parsing
                // we just want to know that there was a combinator
                if (stream.Current.Start >= (stream.Peek(-1).End + 1))
                    Combinator = new DescendantCombinator();
            }

            return Combinator != null;
        }
    }
}
