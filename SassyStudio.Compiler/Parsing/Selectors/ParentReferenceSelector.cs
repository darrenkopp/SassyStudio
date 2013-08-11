using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing.Selectors
{
    public class ParentReferenceSelector : SimpleSelector
    {
        public TokenItem Ampersand { get; protected set; }

        protected override bool ParseSelectorToken(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Ampersand)
                Ampersand = Children.AddCurrentAndAdvance(stream, SassClassifierType.ParentReference);

            return Children.Count > 0;
        }
    }
}
