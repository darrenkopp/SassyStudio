using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    class MixinReference : ComplexItem
    {
        public AtRule Rule { get; set; }
        public MixinName Name { get; set; }
        public TokenItem Semicolon { get; set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, "include") && MixinName.IsValidName(stream.Peek(2)))
            {
                Rule = AtRule.CreateParsed(itemFactory, text, stream);
                Name = MixinName.CreateParsed(itemFactory, text, stream, SassClassifierType.MixinReference);

                Children.Add(Rule);
                Children.Add(Name);

                if (stream.Current.Type == TokenType.Semicolon)
                    Semicolon = Children.AddCurrentAndAdvance(stream);
            }

            return Children.Count > 0;
        }
    }
}
