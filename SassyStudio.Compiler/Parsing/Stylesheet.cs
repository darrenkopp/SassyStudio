using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class Stylesheet : ComplexItem, ISassStylesheet
    {
        public ISassDocument Owner { get; set; }

        public override bool IsUnclosed { get { return false; } }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.StartOfFile)
            {
                stream.Advance();
                while (stream.Current.Type != TokenType.EndOfFile)
                {
                    int position = stream.Position;
                    ParseItem item;
                    if (itemFactory.TryCreateParsedOrDefault(this, text, stream, out item))
                    {
                        Children.Add(item);
                    }
                    else if (stream.Position == position)
                    {
                        // add current token if and only if we didn't move position at all
                        Children.AddCurrentAndAdvance(stream);
                    }
                }
            }

            return Children.Count > 0;
        }
    }
}
