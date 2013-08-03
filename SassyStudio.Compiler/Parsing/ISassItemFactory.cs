using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public interface ISassItemFactory
    {
        ParseItem CreateItem(IItemFactory itemFactory, ITextProvider text, ITokenStream stream, ComplexItem parent, Type type);
    }
}
