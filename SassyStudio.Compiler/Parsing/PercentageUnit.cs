using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class PercentageUnit : SimplexItem
    {
        public TokenItem Value { get; protected set; }
        public TokenItem PercentSign { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Number)
                Value = Children.AddCurrentAndAdvance(stream);

            if (stream.Current.Type == TokenType.PercentSign)
                PercentSign = Children.AddCurrentAndAdvance(stream);

            return Children.Count > 0;
        }
    }
}
