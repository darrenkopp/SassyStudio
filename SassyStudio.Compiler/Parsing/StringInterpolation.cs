using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class StringInterpolation : ComplexItem
    {
        public StringInterpolation()
        {

        }

        public TokenItem OpenInterpolation { get; protected set; }
        public TokenItem CloseInterpolation { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.OpenInterpolation)
            {
                OpenInterpolation = Children.AddCurrentAndAdvance(stream, SassClassifierType.Interpolation);

                while (!IsTerminator(stream.Current.Type))
                {
                    ParseItem item;
                    if (itemFactory.TryCreateParsedOrDefault(this, text, stream, out item))
                        Children.Add(item);
                }

                if (stream.Current.Type == TokenType.CloseInterpolation)
                    CloseInterpolation = Children.AddCurrentAndAdvance(stream, SassClassifierType.Interpolation);
            }

            return Children.Count > 0;
        }

        static bool IsTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.CloseInterpolation:
                    return true;
            }

            return false;
        }
    }
}
