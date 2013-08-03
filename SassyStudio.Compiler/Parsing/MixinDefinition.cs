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
            if (AtRule.IsRule(text, stream, "mixin") && MixinName.IsValidName(stream.Peek(2)))
            {
                Rule = AtRule.CreateParsed(itemFactory, text, stream);
                Name = MixinName.CreateParsed(itemFactory, text, stream, SassClassifierType.MixinDefinition);

                Children.Add(Rule);
                Children.Add(Name);
            }

            return Children.Count > 0;
        }
    }
}
