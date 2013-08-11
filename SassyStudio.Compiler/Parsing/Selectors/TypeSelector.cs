using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing.Selectors
{
    public class TypeSelector : SimpleSelector
    {
        public TokenItem Name { get; protected set; }
        protected override bool ParseSelectorToken(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Identifier)
                Name = Children.AddCurrentAndAdvance(stream, SassClassifierType.ElementName);

            return Children.Count > 0;
        }
    }
}
