using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    class MixinDefinition : ComplexItem
    {
        public AtRule Rule { get; protected set; }
        public MixinName Name { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, "mixin"))
            {
                Rule = AtRule.CreateParsed(itemFactory, text, stream);
                if (Rule != null)
                    Children.Add(Rule);

                Name = MixinName.CreateParsed(itemFactory, text, stream, SassClassifierType.MixinDefinition);
                if (Name != null)
                    Children.Add(Name);
            }

            return Children.Count > 0;
        }
    }
}
