using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing.Selectors
{
    public class UniversalSelector : SimpleSelector
    {
        public TokenItem Asterisk { get; protected set; }

        protected override bool ParseSelectorToken(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Asterisk)
                Asterisk = Children.AddCurrentAndAdvance(stream);

            return Children.Count > 0;
        }
    }
}
