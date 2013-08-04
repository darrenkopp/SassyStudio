using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class ListItem : ComplexItem
    {
        public ParseItem Value { get; protected set; }
        public TokenItem Comma { get; protected set; }
        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            ParseItem item;
            if (itemFactory.TryCreateParsedOrDefault(this, text, stream, out item))
            {
                Value = item;
                Children.Add(item);
            }

            if (stream.Current.Type == TokenType.Comma)
                Comma = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);

            return Children.Count > 0;
        }
    }
}
