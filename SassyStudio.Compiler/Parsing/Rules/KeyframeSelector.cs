using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing.Rules
{
    public class KeyframeSelector : ComplexItem
    {
        public ParseItem AnimationBegin { get; protected set; }
        public TokenItem Comma { get; protected set; }
        public ParseItem AnimationEnd { get; protected set; }
        public RuleBlock Body { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Identifier && IsValidNamedRange(text.GetText(stream.Current.Start, stream.Current.Length)))
            {
                AnimationBegin = Children.AddCurrentAndAdvance(stream, SassClassifierType.Keyword);
            }
            else if (stream.Current.Type == TokenType.Number && stream.Peek(1).Type == TokenType.PercentSign)
            {
                ParseItem begin;
                if (itemFactory.TryCreateParsed<PercentageUnit>(this, text, stream, out begin))
                {
                    AnimationBegin = begin;
                    Children.Add(begin);

                    if (stream.Current.Type == TokenType.Comma)
                        Comma = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);

                    ParseItem end;
                    if (itemFactory.TryCreateParsed<PercentageUnit>(this, text, stream, out end))
                    {
                        AnimationEnd = end;
                        Children.Add(end);
                    }
                }
            }

            if (AnimationBegin != null)
            {
                var block = itemFactory.CreateSpecific<RuleBlock>(this, text, stream);
                if (block.Parse(itemFactory, text, stream))
                {
                    Body = block;
                    Children.Add(block);
                }
            }

            return Children.Count > 0;
        }

        private bool IsValidNamedRange(string name)
        {
            switch (name)
            {
                case "to":
                case "from":
                    return true;
                default:
                    return false;
            }
        }
    }
}
