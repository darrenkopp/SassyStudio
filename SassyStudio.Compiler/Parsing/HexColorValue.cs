using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class HexColorValue : ComplexItem
    {
        public TokenItem Hash { get; protected set; }
        public TokenItem Color { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Hash)
            {
                Hash = Children.AddCurrentAndAdvance(stream, SassClassifierType.HexColor);
                Color = Children.AddCurrentAndAdvance(stream, SassClassifierType.HexColor);
            }

            return Children.Count > 0;
        }
    }
}
