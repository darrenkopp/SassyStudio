using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class PlaceholderSelectorName : SimplexItem
    {
        public PlaceholderSelectorName()
        {
            ClassifierType = SassClassifierType.PlaceholderName;
        }

        public TokenItem Prefix { get; protected set; }
        public TokenItem Name { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.PercentSign && stream.Peek(1).Type == TokenType.Identifier)
            {
                Prefix = Children.AddCurrentAndAdvance(stream);
                Name = Children.AddCurrentAndAdvance(stream);
            }

            return Children.Count > 0;
        }
    }
}
