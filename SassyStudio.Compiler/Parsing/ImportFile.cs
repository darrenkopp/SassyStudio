using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class ImportFile : ComplexItem
    {
        public TokenItem Path { get; protected set; }
        public TokenItem Comma { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.String || stream.Current.Type == TokenType.BadString)
            {
                Path = Children.AddCurrentAndAdvance(stream, SassClassifierType.String);
                if (stream.Current.Type == TokenType.Comma)
                    Comma = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);
            }

            return Children.Count > 0;
        }
    }
}
