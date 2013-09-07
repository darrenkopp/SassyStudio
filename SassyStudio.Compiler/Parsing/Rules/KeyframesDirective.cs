using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing.Rules
{
    public class KeyframesDirective : AtRuleDirective
    {
        protected override string RuleName { get { return "keyframes"; } }

        public TokenItem AnimationName { get; protected set; }

        public KeyframeRuleBlock KeyframeList { get; protected set; }

        protected override void ParseDirective(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Identifier)
                AnimationName = Children.AddCurrentAndAdvance(stream, SassClassifierType.Default);
        }

        protected override void ParseBlock(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            var block = itemFactory.CreateSpecific<KeyframeRuleBlock>(this, text, stream);
            if (block.Parse(itemFactory, text, stream))
            {
                KeyframeList = block;
                Children.Add(block);
            }
        }
    }
}
