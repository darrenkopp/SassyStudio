using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class WhileLoopDirective : ControlDirective
    {
        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, "while"))
            {
                ParseRule(itemFactory, text, stream);

                while (!IsConditionTerminator(stream.Current.Type))
                {
                    ParseItem item;
                    if (itemFactory.TryCreateParsedOrDefault(this, text, stream, out item))
                        Children.Add(item);
                }

                ParseBody(itemFactory, text, stream);
            }

            return Children.Count > 0;
        }

        private bool IsConditionTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.OpenCurlyBrace:
                    return true;
                default:
                    return false;
            }
        }
    }
}
