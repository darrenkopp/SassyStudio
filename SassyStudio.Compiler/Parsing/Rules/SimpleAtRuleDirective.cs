using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public abstract class SimpleAtRuleDirective : ComplexItem
    {
        public AtRule Rule { get; protected set; }
        public TokenItem Semicolon { get; protected set; }

        protected abstract string RuleName { get; }
        protected abstract void ParseDirective(IItemFactory itemFactory, ITextProvider text, ITokenStream stream);

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, RuleName))
            {
                Rule = AtRule.CreateParsed(itemFactory, text, stream);
                if (Rule != null)
                    Children.Add(Rule);

                ParseDirective(itemFactory, text, stream);

                if (stream.Current.Type == TokenType.Semicolon)
                    Semicolon = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);
            }

            return Children.Count > 0;
        }
    }
}
