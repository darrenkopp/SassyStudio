using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class IdName : ComplexItem
    {
        public TokenItem Hash { get; protected set; }
        public TokenItem Name { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Hash && stream.Peek(1).Type == TokenType.Identifier)
            {
                Hash = Children.AddCurrentAndAdvance(stream, SassClassifierType.IdName);
                Name = Children.AddCurrentAndAdvance(stream, SassClassifierType.IdName);
            }

            return Children.Count > 0;
        }
    }
}
