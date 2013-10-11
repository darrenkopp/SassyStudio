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
                { typeof(XmlDocumentationTag), CreateDocumentationTag },
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
            if (parent is MixinReference || parent is SystemFunctionReference || parent is UserFunctionReference)
                if (VariableName.IsVariable(text, stream) && stream.Peek(2).Type == TokenType.Colon)
                    return new NamedFunctionArgument();

            return null;
        }

        static ParseItem CreateSimpleSelector(ComplexItem parent, IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            switch (stream.Current.Type)
            {
                case TokenType.Ampersand: return new ParentReferenceSelector();
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

        static ParseItem CreateDocumentationTag(ComplexItem parent, IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (!(parent is XmlDocumentationComment))
                return null;

            if (stream.Current.Type == TokenType.LessThan)
            {
                var next = stream.Peek(1);
                if (next.Type != TokenType.Identifier)
                    return null;

                switch (text.GetText(next.Start, next.Length))
                {
                    case "reference": return new FileReferenceTag();
                }
            }

            return null;
        }
    }
}
