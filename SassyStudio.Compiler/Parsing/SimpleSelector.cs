using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class SimpleSelector : ComplexItem
    {
        public SimpleSelector()
        {
            SelectorParts = new ParseItemList();
        }

        public ParseItemList SelectorParts { get; protected set; }
        public TokenItem Comma { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            while (!IsTerminator(stream.Current.Type))
            {
                ParseItem part;
                if (!itemFactory.TryCreateParsedOrDefault(this, text, stream, out part))
                    break;

                SelectorParts.Add(part);
                Children.Add(part);
            }

            return Children.Count > 0;
        }

        private bool IsTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.Comma:
                case TokenType.OpenCurlyBrace:
                    return true;
                default:
                    return false;
            }
        }
    }
}
