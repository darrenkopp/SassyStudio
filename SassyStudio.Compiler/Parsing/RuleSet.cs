using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class RuleSet : ComplexItem
    {
        readonly List<Selector> _Selectors = new List<Selector>(0);

        public IReadOnlyCollection<Selector> Selectors { get { return _Selectors; } }
        public RuleBlock Block { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            while (!IsSelectorTerminator(stream.Current.Type))
            {
                var selector = itemFactory.CreateSpecific<Selector>(this, text, stream);
                if (!selector.Parse(itemFactory, text, stream))
                    break;

                _Selectors.Add(selector);
                Children.Add(selector);
            }

            var block = itemFactory.CreateSpecific<RuleBlock>(this, text, stream);
            if (block.Parse(itemFactory, text, stream))
            {
                Block = block;
                Children.Add(block);
            }

            return Children.Count > 0;
        }

        private bool IsSelectorTerminator(TokenType type)
        {
            return type == TokenType.EndOfFile || type == TokenType.OpenCurlyBrace;
        }

        public static bool IsRuleSet(ITokenStream stream)
        {
            int start = stream.Position;
            var lastToken = stream.Current;
            bool isValid = false;
            while (stream.Current.Type != TokenType.EndOfFile)
            {
                var nextToken = stream.Advance();

                // if we have colon that isn't immediately followed by something, can't be a selector part
                if (lastToken.Type == TokenType.Colon && nextToken.Start != lastToken.End)
                {
                    isValid = false;
                    break;
                }

                if (stream.Current.Type == TokenType.OpenCurlyBrace)
                {
                    isValid = true;
                    break;
                }

                lastToken = stream.Current;
            }

            stream.SeekTo(start);
            return isValid;
        }
    }
}
