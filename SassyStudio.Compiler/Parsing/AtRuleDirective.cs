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

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, RuleName))
            {
                Rule = AtRule.CreateParsed(itemFactory, text, stream);
                if (Rule != null)
                    Children.Add(Rule);

                var block = itemFactory.CreateSpecific<RuleBlock>(this, text, stream);
                if (block.Parse(itemFactory, text, stream))
                {
                    Body = block;
                    Children.Add(block);
                }
            }

            return Children.Count > 0;
        }
    }
}
