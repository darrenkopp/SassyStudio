using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    class CharsetDirective : SimpleAtRuleDirective
    {
        protected override string RuleName { get { return "charset"; } }

        public ParseItem CharacterSet { get; protected set; }

        protected override void ParseDirective(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            switch (stream.Current.Type)
            {
                case TokenType.String:
                    CharacterSet = itemFactory.CreateSpecificParsed<StringValue>(this, text, stream);
                    if (CharacterSet != null)
                        Children.Add(CharacterSet);
                    break;
                case TokenType.Identifier:
                    CharacterSet = Children.AddCurrentAndAdvance(stream);
                    break;
            }   
        }
    }
}
