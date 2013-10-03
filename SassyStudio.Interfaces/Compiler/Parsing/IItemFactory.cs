using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public interface IItemFactory
    {
        ParseItem Create<T>(ComplexItem parent, ITextProvider text, ITokenStream stream) where T : ParseItem, new();
        T CreateSpecific<T>(ComplexItem parent, ITextProvider text, ITokenStream stream) where T : ParseItem, new();
        T CreateSpecificParsed<T>(ComplexItem parent, ITextProvider text, ITokenStream stream) where T : ParseItem, new();
        bool TryCreate(ComplexItem parent, ITextProvider text, ITokenStream stream, out ParseItem item);
        bool TryCreateParsed<T>(ComplexItem parent, ITextProvider text, ITokenStream stream, out ParseItem item) where T : ParseItem;
        bool TryCreateParsedOrDefault(ComplexItem parent, ITextProvider text, ITokenStream stream, out ParseItem item);
    }
}
