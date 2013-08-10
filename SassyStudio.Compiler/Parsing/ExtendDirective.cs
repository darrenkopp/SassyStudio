using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class ExtendDirective : ComplexItem
    {
        public AtRule Rule { get; protected set; }
        public ClassName ExtensionClass { get; protected set; }
        public OptionalModifier Modifier { get; protected set; }
        public TokenItem Semicolon { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, "extend"))
            {
                Rule = AtRule.CreateParsed(itemFactory, text, stream);
                if (Rule != null)
                    Children.Add(Rule);

                var name = new ClassName();
                if (name.Parse(itemFactory, text, stream))
                {
                    ExtensionClass = name;
                    Children.Add(name);
                }

                if (stream.Current.Type == TokenType.Bang)
                {
                    var modifier = new OptionalModifier();
                    if (modifier.Parse(itemFactory, text, stream))
                    {
                        Modifier = modifier;
                        Children.Add(modifier);
                    }
                }

                if (stream.Current.Type == TokenType.Semicolon)
                    Semicolon = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);
            }

            return Children.Count > 0;
        }
    }
}
