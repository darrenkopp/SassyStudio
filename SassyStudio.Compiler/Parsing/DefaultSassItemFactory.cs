using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class DefaultSassItemFactory : ISassItemFactory
    {
        delegate ParseItem CreateParseItem(ComplexItem parent, IItemFactory itemFactory, ITextProvider text, ITokenStream stream);
        static readonly IDictionary<Type, CreateParseItem> Registry = CreateRegistry();

        private static IDictionary<Type, CreateParseItem> CreateRegistry()
        {
            return new Dictionary<Type, CreateParseItem>
            {
                { typeof(FunctionArgument), CreateFunctionArgument }
            };
        }

        public ParseItem CreateItem(IItemFactory itemFactory, ITextProvider text, ITokenStream stream, ComplexItem parent, Type type)
        {
            CreateParseItem handler;
            if (Registry.TryGetValue(type, out handler))
                return handler(parent, itemFactory, text, stream);

            return null;
        }

        static ParseItem CreateFunctionArgument(ComplexItem parent, IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (parent is MixinReference && VariableName.IsVariable(text, stream))
                return new NamedFunctionArgument();

            return null;
        }
    }
}
