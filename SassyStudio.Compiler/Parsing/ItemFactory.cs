using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    class ItemFactory : IItemFactory
    {
        private readonly ISassItemFactory ExternalItemFactory;
        public ItemFactory(ISassItemFactory externalItemFactory)
        {
            ExternalItemFactory = externalItemFactory ?? new DefaultSassItemFactory();
        }

        public ParseItem Create<T>(ComplexItem parent, ITextProvider text, ITokenStream stream) where T : ParseItem
        {            
            var type = typeof(T);

            // attempt to create with external factory
            ParseItem item = (ExternalItemFactory != null)
                ? ExternalItemFactory.CreateItem(this, text, stream, parent, type)
                : null;

            if (item == null)
                item = Activator.CreateInstance(type, true) as ParseItem;

            return item;
        }

        public T CreateSpecific<T>(ComplexItem parent, ITextProvider text, ITokenStream stream) where T : ParseItem
        {
            return (T)Create<T>(parent, text, stream);
        }

        public bool TryCreate(ComplexItem parent, ITextProvider text, ITokenStream stream, out ParseItem item)
        {
            item = null;
            switch (stream.Current.Type)
            {
                case TokenType.String:
                case TokenType.BadString:
                    item = new TokenItem(SassClassifierType.String);
                    break;
                case TokenType.OpenCssComment:
                    item = new CssComment();
                    break;
                case TokenType.CppComment:
                    item = new CppComment();
                    break;
                case TokenType.EndOfFile:
                    item = null;
                    return false;
                case TokenType.StartOfFile:
                    item = Create<Stylesheet>(parent, text, stream);
                    break;
            }

            return item != null;
        }
    }
}
