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

        public static bool IsValidRuleSet(ITokenStream stream)
        {
            int position = stream.Position;
            try
            {
                var previous = stream.Current;
                while (stream.Advance().Type != TokenType.EndOfFile)
                {
                    var curent = stream.Current;
                    // selector body
                    if (curent.Type == TokenType.OpenCurlyBrace) return true;
                    // parent terminator
                    if (curent.Type == TokenType.CloseCurlyBrace) return false;
                    // property terminator
                    if (curent.Type == TokenType.Semicolon) return false;
                    if (curent.Type == TokenType.Colon)
                    {
                        var next = stream.Advance();
                        // if space after colon, then we are in a property
                        if (curent.End != next.Start) return false;
                    }

                    // check for space
                    if (curent.Start != previous.End) return true;
                    
                    previous = stream.Current;
                }
            }
            finally
            {
                stream.SeekTo(position);
            }

            return false;
        }
    }
}
