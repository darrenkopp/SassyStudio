using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing.Selectors
{
    public class DescendantCombinator : SelectorCombinator
    {
        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            // this represents 
            return stream.Current.Start > stream.Peek(-1).End;
        }
    }
}
