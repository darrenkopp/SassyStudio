using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class EachLoopDirective : ControlDirective
    {
        readonly List<ListItem> _Items = new List<ListItem>();
        public IReadOnlyCollection<ListItem> Items { get { return _Items; } }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, "each"))
            {
                ParseRule(itemFactory, text, stream);
                while (!IsListTerminator(stream.Current.Type))
                {
                    var item = itemFactory.CreateSpecific<ListItem>(this, text, stream);
                    if (item != null && item.Parse(itemFactory, text, stream))
                    {
                        _Items.Add(item);
                        Children.Add(item);
                    }
                    else
                    {
                        // bad news bears
                        Children.AddCurrentAndAdvance(stream);
                    }
                }

                ParseBody(itemFactory, text, stream);
            }

            return Children.Count > 0;
        }

        private bool IsListTerminator(TokenType type)
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

        public override void Freeze()
        {
            base.Freeze();
            _Items.TrimExcess();
        }
    }
}
