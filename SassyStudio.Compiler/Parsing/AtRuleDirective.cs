using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public abstract class AtRuleDirective : ComplexItem
    {
        public AtRule Rule { get; protected set; }
        public RuleBlock Body { get; protected set; }

        protected abstract string RuleName { get; }

        protected virtual void ParseDirective(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
        }

        protected virtual void ParseBlock(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            var block = itemFactory.CreateSpecific<RuleBlock>(this, text, stream);
            if (block.Parse(itemFactory, text, stream))
            {
                Body = block;
                Children.Add(block);
            }
        }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, RuleName))
            {
                Rule = AtRule.CreateParsed(itemFactory, text, stream);
                if (Rule != null)
                    Children.Add(Rule);

                ParseDirective(itemFactory, text, stream);
                ParseBlock(itemFactory, text, stream);
            }

            return Children.Count > 0;
        }
    }
}
