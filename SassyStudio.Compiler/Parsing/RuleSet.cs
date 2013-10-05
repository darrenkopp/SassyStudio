using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class RuleSet : ComplexItem
    {
        readonly List<SelectorGroup> _Selectors = new List<SelectorGroup>(0);

        public IReadOnlyCollection<SelectorGroup> Selectors { get { return _Selectors; } }
        public RuleBlock Block { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            while (!IsSelectorTerminator(stream.Current.Type))
            {
                var selector = itemFactory.CreateSpecific<SelectorGroup>(this, text, stream);
                if (!selector.Parse(itemFactory, text, stream))
                    break;

                _Selectors.Add(selector);
                Children.Add(selector);

                if (stream.Current.Type == TokenType.Comma)
                    Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);
            }

            if (stream.Current.Type == TokenType.OpenCurlyBrace)
            {
                var block = itemFactory.CreateSpecific<RuleBlock>(this, text, stream);
                if (block.Parse(itemFactory, text, stream))
                {
                    Block = block;
                    Children.Add(block);
                }
            }

            return Children.Count > 0;
        }

        private bool IsSelectorTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.OpenCurlyBrace:
                case TokenType.CloseCurlyBrace:
                    return true;
                default:
                    return false;
            }
        }
    }
}
