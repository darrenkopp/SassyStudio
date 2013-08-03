using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class FunctionArgument : ComplexItem
    {
        public FunctionArgument()
        {
            Values = new ParseItemList();
        }

        public ParseItemList Values { get; protected set; }
        public TokenItem Comma { get; set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            while (!IsTerminator(stream))
            {
                ParseItem item;
                if (!itemFactory.TryCreate(this, text, stream, out item) || !item.Parse(itemFactory, text, stream))
                {
                    item = new TokenItem();
                    item.Parse(itemFactory, text, stream);
                }

                Values.Add(item);
                Children.Add(item);
            }

            if (stream.Current.Type == TokenType.Comma)
                Comma = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);

            return Children.Count > 0;
        }

        private bool IsTerminator(ITokenStream stream)
        {
            switch (stream.Current.Type)
            {
                case TokenType.EndOfFile:
                case TokenType.Comma:
                case TokenType.CloseFunctionBrace:
                case TokenType.Semicolon:
                    return true;
            }

            return false;
        }

        public override void Freeze()
        {
            base.Freeze();
            if (Values.Count > 0)
                Values.TrimExcess();
        }
    }
}
