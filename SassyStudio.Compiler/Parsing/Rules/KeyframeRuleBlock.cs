using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing.Rules
{
    public class KeyframeRuleBlock : BlockItem
    {
        readonly List<KeyframeSelector> _Selectors = new List<KeyframeSelector>(0);
        public IReadOnlyList<KeyframeSelector> Selectors { get { return _Selectors; } }
 
        protected override void ParseBody(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            while (!IsTerminator(stream.Current.Type))
            {
                var selector = itemFactory.CreateSpecific<KeyframeSelector>(this, text, stream);
                if (!selector.Parse(itemFactory, text, stream))
                    break;

                Children.Add(selector);
                _Selectors.Add(selector);
            }
        }

        public override void Freeze()
        {
            _Selectors.TrimExcess();
            base.Freeze();
        }
    }
}
