using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SassyStudio.Compiler.Lexing;

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
                case TokenType.Hash:
                    item = CreateHash(parent, text, stream);
                    break;
                case TokenType.ParentReference:
                    item = new TokenItem(SassClassifierType.ParentReference);
                    break;
                case TokenType.OpenCurlyBrace:
                    item = new BlockItem();
                    break;
                case TokenType.Period:
                    item = CreatePeriod(parent, text, stream);
                    break;
                case TokenType.Identifier:
                case TokenType.OpenInterpolation:
                    item = CreateIdentLikeItem(parent, text, stream);
                    break;
            }

            return item != null;
        }

        public bool TryCreateParsedOrDefault(ComplexItem parent, ITextProvider text, ITokenStream stream, out ParseItem item)
        {
            if (!TryCreate(parent, text, stream, out item))
                item = new TokenItem();

            if (!item.Parse(this, text, stream))
                item = null;

            return item != null;
        }

        private ParseItem CreateIdentLikeItem(ComplexItem parent, ITextProvider text, ITokenStream stream)
        {
            if (IsInValueContext(parent))
                return CreateIdentLikeValue(parent, text, stream);

            // check for list of selectors
            if (RuleSet.IsRuleSet(stream))
                return new RuleSet();

            // check for property declaration
            if (PropertyDeclaration.IsDeclaration(stream))
                return new PropertyDeclaration();

            if (stream.Current.Type == TokenType.OpenInterpolation)
                return new StringInterpolation();

            // normal identifier
            return new TokenItem();
        }

        private bool IsInValueContext(ComplexItem parent)
        {
            if (parent == null)
                return false;

            return !(
                   parent is Stylesheet
                || parent is BlockItem
            );
        }

        private ParseItem CreateIdentLikeValue(ComplexItem parent, ITextProvider text, ITokenStream stream)
        {
            switch (stream.Current.Type)
            {
                case TokenType.OpenInterpolation:
                    return new StringInterpolation();
                default:
                    return new TokenItem();
            }
        }

        private ParseItem CreateFunction(ComplexItem parent, ITextProvider text, ITokenStream stream)
        {
            if (Function.IsWellKnownFunction(text, stream))
                return new SystemFunctionReference();

            return new UserFunctionReference();
        }

        private ParseItem CreateBang(ComplexItem parent, ITextProvider text, ITokenStream stream)
        {
            if (ImportanceModifier.IsImportanceModifier(text, stream))
                return new ImportanceModifier();

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
                    case "extend": return new ExtendDirective();
                    case "for": return new ForLoopDirective();
                    case "while": return new WhileLoopDirective();
                    case "each": return new EachLoopDirective();
                    case "if":
                    case "else":
                    case "else if": return new ConditionalControlDirective();
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

        private ParseItem CreatePeriod(ComplexItem parent, ITextProvider text, ITokenStream stream)
        {
            if (stream.Peek(1).Type == TokenType.Identifier)
                return new ClassName();

            return new TokenItem();
        }

        private ParseItem CreateHash(ComplexItem parent, ITextProvider text, ITokenStream stream)
        {
            var value = stream.Peek(1);
            if (value.Type == TokenType.Identifier)
            {
                if (parent is Selector || parent is SimpleSelector)
                    return new IdName();

                if ((value.Length == 3 || value.Length == 6) && IsHex(text, value))
                    return new HexColorValue();
            }

            return new TokenItem();
        }

        private bool IsHex(ITextProvider text, Token value)
        {
            var hex = text.GetText(value.Start, value.Length);
            for (int i = 0; i < hex.Length; i++)
            {
                char c = hex[i];
                if (!(char.IsDigit(c) || ((c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'))))
                    return false;
            }

            return true;
        }
    }
}
