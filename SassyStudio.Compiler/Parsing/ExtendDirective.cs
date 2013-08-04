using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    class ExtendDirective : ComplexItem
    {
        public AtRule Rule { get; protected set; }
        public ClassName ExtensionClass { get; set; }
        public TokenItem Semicolon { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, "extend"))
            {
                Rule = AtRule.CreateParsed(itemFactory, text, stream);

                var name = new ClassName();
                if (name.Parse(itemFactory, text, stream))
                {
                    ExtensionClass = name;
                    Children.Add(name);
                }

                if (stream.Current.Type == TokenType.Semicolon)
                    Semicolon = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);
            }

            return Children.Count > 0;
        }
    }
}
