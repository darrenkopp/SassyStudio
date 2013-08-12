using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class AtRule : SimplexItem
    {
        public AtRule()
        {
            ClassifierType = SassClassifierType.Keyword;
        }

        public TokenItem At { get; protected set; }
        public TokenItem Name { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.At && stream.Peek(1).Type == TokenType.Identifier)
            {
                At = Children.AddCurrentAndAdvance(stream);
                Name = Children.AddCurrentAndAdvance(stream);
            }

            return Children.Count > 0;
        }

        public static bool IsRule(ITextProvider text, ITokenStream stream, string name)
        {
            if (stream.Current.Type == TokenType.At)
            {
                var nameToken = stream.Peek(1);
                if (nameToken.Type == TokenType.Identifier)
                    return text.GetText(nameToken.Start, name.Length) == name;
            }

            return false;
        }

        public static AtRule CreateParsed(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            var rule = new AtRule();
            if (rule.Parse(itemFactory, text, stream))
                return rule;

            return null;
        }
    }
}
