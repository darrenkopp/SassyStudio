using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SassyStudio.Compiler.Parsing.Selectors;

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
                { typeof(FunctionArgument), CreateFunctionArgument },
                { typeof(SimpleSelector), CreateSimpleSelector },
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

        static ParseItem CreateSimpleSelector(ComplexItem parent, IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            switch (stream.Current.Type)
            {
                case TokenType.Asterisk: return new UniversalSelector();
                case TokenType.Period: return new ClassSelector();
                case TokenType.Hash: return new IdSelector();
                case TokenType.Identifier: return new TypeSelector();
                case TokenType.OpenBrace: return new AttributeSelector();
                case TokenType.DoubleColon: return new PseudoElementSelector();
                case TokenType.PercentSign: return new ExtendOnlySelector();
            }

            if (stream.Current.Type == TokenType.Colon)
            {
                var next = stream.Peek(1);
                switch (next.Type)
                {
                    case TokenType.Identifier: return new PseudoClassSelector();
                    case TokenType.Function: return new PseudoFunctionSelector();
                }
            }

            return null;
        }
    }
}
