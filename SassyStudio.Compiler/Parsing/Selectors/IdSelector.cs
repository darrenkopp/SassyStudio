using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing.Selectors
{
    public class IdSelector : SimpleSelector
    {
        public IdSelector()
        {
            ClassifierType = SassClassifierType.IdName;
        }

        public IdName Name { get; protected set; }

        protected override bool ParseSelectorToken(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            var name = itemFactory.CreateSpecific<IdName>(this, text, stream);
            if (name.Parse(itemFactory, text, stream))
            {
                Name = name;
                Children.Add(name);
            }

            return Children.Count > 0;
        }
    }
}
