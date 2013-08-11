using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing.Selectors
{
    public class StringInterpolationSelector : SimpleSelector
    {
        public StringInterpolation Interpolation { get; protected set; }

        protected override bool ParseSelectorToken(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.OpenInterpolation)
            {
                var interpolation = itemFactory.CreateSpecific<StringInterpolation>(this, text, stream);
                if (interpolation.Parse(itemFactory, text, stream))
                {
                    Interpolation = interpolation;
                    Children.Add(interpolation);
                }
            }

            return Children.Count > 0;
        }
    }
}
