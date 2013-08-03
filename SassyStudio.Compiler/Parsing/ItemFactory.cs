using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public ParseItem Create<T>(ComplexItem parent, ITextProvider text, ITokenStream stream) where T : ParseItem, new()
        {
            var type = typeof(T);

            // attempt to create with external factory
            ParseItem item = (ExternalItemFactory != null)
                ? ExternalItemFactory.CreateItem(this, text, stream, parent, type)
                : null;

            if (item == null)
                item = new T();

            return item;
        }

        public T CreateSpecific<T>(ComplexItem parent, ITextProvider text, ITokenStream stream) where T : ParseItem, new()
        {
            return (T)Create<T>(parent, text, stream);
        }

        public bool TryCreate(ComplexItem parent, ITextProvider text, ITokenStream stream, out ParseItem item)
        {
            item = null;
            switch (stream.Current.Type)
            {
                case TokenType.EndOfFile:
                    item = null;
                    return false;
                case TokenType.StartOfFile:
                    item = Create<Stylesheet>(parent, text, stream);
                    break;
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
                case TokenType.OpenHtmlComment:
                    item = new HtmlComment();
                    break;
                case TokenType.At:
                    item = CreateAtRule(parent, text, stream);
                    break;
                case TokenType.Dollar:
                    item = CreateVariableDefinitionOrReference(parent, text, stream);
                    break;
                case TokenType.Bang:
                    item = CreateBang(parent, text, stream);
                    break;
                case TokenType.Function:
                    item = CreateFunction(parent, text, stream);
                    break;
                case TokenType.ParentReference:
                    item = new TokenItem(SassClassifierType.ParentReference);
                    break;
                case TokenType.OpenInterpolation:
                    item = new StringInterpolation();
                    break;
            }

            return item != null;
        }

        private ParseItem CreateFunction(ComplexItem parent, ITextProvider text, ITokenStream stream)
        {
            if (Function.IsWellKnownFunction(text, stream))
                return new SystemFunctionReference();

            return new UserFunctionReference();
        }

        private ParseItem CreateBang(ComplexItem parent, ITextProvider text, ITokenStream stream)
        {
            if (VariableName.IsVariable(text, stream))
                return CreateVariableDefinitionOrReference(parent, text, stream);

            return new TokenItem();
        }

        private ParseItem CreateAtRule(ComplexItem parent, ITextProvider text, ITokenStream stream)
        {
            var nameToken = stream.Peek(1);
            if (nameToken.Type == TokenType.Identifier)
            {
                var ruleName = text.GetText(nameToken.Start, nameToken.Length);
                switch (ruleName)
                {
                    case "mixin": return new MixinDefinition();
                    case "include": return new MixinReference();
                    case "function": return new UserFunctionDefinition();
                    case "import": return new ImportDirective();
                    default: return new AtRule();
                }
            }

            return new TokenItem();
        }

        private ParseItem CreateVariableDefinitionOrReference(ComplexItem parent, ITextProvider text, ITokenStream stream)
        {
            if (stream.Peek(1).Type == TokenType.Identifier)
            {
                if (stream.Peek(2).Type == TokenType.Colon)
                    return new VariableDefinition();

                return new VariableReference();
            }

            return new TokenItem();
        }
    }
}
