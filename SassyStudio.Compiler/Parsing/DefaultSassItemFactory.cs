using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class DefaultSassItemFactory : ISassItemFactory
    {
        public ParseItem CreateItem(IItemFactory itemFactory, ITextProvider text, ITokenStream stream, ComplexItem parent, Type type)
        {
            return null;
        }
    }
}
