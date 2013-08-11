using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class SelectorGroup : ComplexItem
    {
        readonly List<SimpleSelector> _SimpleSelectors = new List<SimpleSelector>(0);

        public IReadOnlyList<SimpleSelector> SimpleSelectors { get { return _SimpleSelectors; } }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Comma)
                Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);

            while (!IsSelectorTerminator(stream.Current.Type))
            {
                ParseItem item;
                if (itemFactory.TryCreateParsed<SimpleSelector>(this, text, stream, out item))
                {
                    Children.Add(item);
                    if (item is SimpleSelector)
                        _SimpleSelectors.Add(item as SimpleSelector);
                }
                else
                {
                    // swallow up unknown and keep going
                    Children.AddCurrentAndAdvance(stream);
                }
            }

            return Children.Count > 0;
        }

        public override void Freeze()
        {
            base.Freeze();
            _SimpleSelectors.TrimExcess();
        }

        static bool IsSelectorTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.Comma:
                case TokenType.OpenCurlyBrace:
                case TokenType.Semicolon:
                    return true;
                default:
                    return false;
            }
        }
    }
}
