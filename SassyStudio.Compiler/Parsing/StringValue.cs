using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class StringValue : SimplexItem
    {
        public StringValue()
        {
            ClassifierType = SassClassifierType.String;    
        }

        public TokenItem Value { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.String || stream.Current.Type == TokenType.BadString)
                Value = Children.AddCurrentAndAdvance(stream, SassClassifierType.String);

            return Children.Count > 0;
        }
    }
}
