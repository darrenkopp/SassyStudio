using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class Selector : ComplexItem
    {
        readonly List<SimpleSelector> _Subselectors = new List<SimpleSelector>(0);

        public IReadOnlyCollection<SimpleSelector> Subselectors { get { return _Subselectors; } }
        public TokenItem Comma { get; protected set; }
        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (!IsSelectorTerminator(stream.Current.Type))
            {
                while (!IsSelectorTerminator(stream.Current.Type))
                {
                    var part = itemFactory.CreateSpecific<SimpleSelector>(this, text, stream);
                    if (!part.Parse(itemFactory, text, stream))
                        break;

                    _Subselectors.Add(part);
                    Children.Add(part);
                }

                if (stream.Current.Type == TokenType.Comma)
                    Comma = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);
            }

            return Children.Count > 0;
        }

        public override void Freeze()
        {
            base.Freeze();
            _Subselectors.TrimExcess();
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
