using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class FunctionArgument : ComplexItem
    {
        public ParseItemList Values { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            while (stream.Current.Type != TokenType.EndOfFile && stream.Current.Type != TokenType.Comma)
            {
                ParseItem item;
                if (!itemFactory.TryCreate(this, text, stream, out item) || !item.Parse(itemFactory, text, stream))
                    break;

                Values.Add(item);
            }

            return Children.Count > 0;
        }
    }
}
