using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing.Selectors
{
    public class ExtendOnlySelector : SimpleSelector
    {
        public PlaceholderSelectorName Name { get; protected set; }

        protected override bool ParseSelectorToken(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            var name = new PlaceholderSelectorName();
            if (name.Parse(itemFactory, text, stream))
            {
                Name = name;
                Children.Add(name);
                return true;
            }

            return false;
        }
    }
}
