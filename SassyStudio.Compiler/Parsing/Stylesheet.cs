using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class Stylesheet : ComplexItem
    {

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.StartOfFile)
            {
                stream.Advance();
                while (stream.Current.Type != TokenType.EndOfFile)
                {
                    int position = stream.Position;
                    // try to create next item, falling back to token item
                    ParseItem item;
                    if (!itemFactory.TryCreate(this, text, stream, out item) || !item.Parse(itemFactory, text, stream))
                    {
                        // if we failed to parse, but moved, then reset and just add normal token
                        if (stream.Position != position)
                            stream.SeekTo(position);

                        item = new TokenItem();
                        item.Parse(itemFactory, text, stream);
                    }

                    Children.Add(item);
                }
            }

            return Children.Count > 0;
        }
    }
}
