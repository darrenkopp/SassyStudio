using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing.Selectors
{
    public class PseudoElementSelector : SimpleSelector
    {
        public TokenItem Prefix { get; protected set; }
        public TokenItem Name { get; protected set; }

        protected override bool ParseSelectorToken(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.DoubleColon && stream.Peek(1).Type == TokenType.Identifier)
            {
                Prefix = Children.AddCurrentAndAdvance(stream, SassClassifierType.PseudoElement);
                Name = Children.AddCurrentAndAdvance(stream, SassClassifierType.PseudoElement);
            }

            return Children.Count > 0;
        }
    }
}
