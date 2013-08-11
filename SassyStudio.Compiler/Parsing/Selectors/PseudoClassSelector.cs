using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing.Selectors
{
    public class PseudoClassSelector : SimpleSelector
    {
        public PseudoClassSelector()
        {
            ClassifierType = SassClassifierType.PseudoClass;
        }

        public TokenItem Prefix { get; protected set; }
        public TokenItem ClassName { get; protected set; }

        protected override bool ParseSelectorToken(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Colon && stream.Peek(1).Type == TokenType.Identifier)
            {
                Prefix = Children.AddCurrentAndAdvance(stream);
                ClassName = Children.AddCurrentAndAdvance(stream);
            }

            return Children.Count > 0;
        }
    }
}
