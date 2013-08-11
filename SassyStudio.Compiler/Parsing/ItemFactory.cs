using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SassyStudio.Compiler.Lexing;
using SassyStudio.Compiler.Parsing.Selectors;

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

        public bool TryCreateParsed<T>(ComplexItem parent, ITextProvider text, ITokenStream stream, out ParseItem item) where T : ParseItem
        {
            item = null;

            if (ExternalItemFactory != null)
                item = ExternalItemFactory.CreateItem(this, text, stream, parent, typeof(T));

            if (item == null && TryCreate(parent, text, stream, out item))
            {
                if (!(item is T))
                    item = null;
            }

            if (item != null)
                return item.Parse(this, text, stream);

            return false;
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
                case TokenType.GreaterThan:
                case TokenType.Plus:
                case TokenType.Tilde:
                case TokenType.Colon:
                case TokenType.DoubleColon:
                case TokenType.Ampersand:
                case TokenType.OpenBrace:
                case TokenType.Hash:
                case TokenType.Period:
                case TokenType.Identifier:
                case TokenType.OpenInterpolation:
                    item = CreateBestFittingItem(parent, text, stream);
                    break;
            }

            return item != null;
        }

        private ParseItem CreateBestFittingItem(ComplexItem parent, ITextProvider text, ITokenStream stream)
        {
            // handle selectors
            if (parent is SelectorGroup)
                return CreateSelectorComponent(parent, text, stream);

            // handle possible property declaration
            if (parent is RuleBlock && PropertyDeclaration.IsDeclaration(stream))
                return new PropertyDeclaration();

            if ((parent is Stylesheet || parent is RuleBlock) && IsRuleSet(parent, stream))
                return new RuleSet();

            return CreateValueItem(parent, text, stream);
        }

        private ParseItem CreateValueItem(ComplexItem parent, ITextProvider text, ITokenStream stream)
        {
            switch (stream.Current.Type)
            {
                case TokenType.OpenInterpolation: return new StringInterpolation();
                case TokenType.Hash: return CreateHash(parent, text, stream);
            }

            // there isn't a more specific representation of token, so just emit normal token item
            return new TokenItem();
        }

        private ParseItem CreateSelectorComponent(ComplexItem parent, ITextProvider text, ITokenStream stream)
        {
            switch (stream.Current.Type)
            {
                case TokenType.Asterisk: return new UniversalSelector();
                case TokenType.Period: return new ClassSelector();
                case TokenType.Hash: return new IdSelector();
                case TokenType.Identifier: return new TypeSelector();
                case TokenType.OpenBrace: return new AttributeSelector();
                case TokenType.DoubleColon: return new PseudoElementSelector();
                case TokenType.GreaterThan: return new ChildCombinator();
                case TokenType.Plus: return new AdjacentSiblingCombinator();
                case TokenType.Tilde: return new GeneralSiblingCombinator();
                case TokenType.Ampersand: return new ParentReferenceSelector();
                case TokenType.OpenInterpolation: return new StringInterpolationSelector();
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

        public bool TryCreateParsedOrDefault(ComplexItem parent, ITextProvider text, ITokenStream stream, out ParseItem item)
        {
            if (!TryCreate(parent, text, stream, out item))
                item = new TokenItem();

            if (!item.Parse(this, text, stream))
                item = null;

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
            if (BangModifier.IsValidModifier(text, stream.Current, "default"))
                return new DefaultModifier();

            if (BangModifier.IsValidModifier(text, stream.Current, "important"))
                return new ImportanceModifier();

            if (BangModifier.IsValidModifier(text, stream.Current, "optional"))
                return new OptionalModifier();

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
                    case "content": return new ContentDirective();
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
            var name = stream.Peek(1);
            if (name.Type == TokenType.Identifier && stream.Current.End == name.Start)
            {
                var assignment = stream.Peek(2);
                if (assignment.Type == TokenType.Colon || assignment.Type == TokenType.Equal)
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
            if (value.Type == TokenType.Identifier && (value.Length == 3 || value.Length == 6) && IsHex(text, value))
                return new HexColorValue();

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

        static bool IsRuleSet(ComplexItem parent, ITokenStream stream)
        {
            bool validStart = false;
            switch (stream.Current.Type)
            {
                case TokenType.Asterisk:
                case TokenType.Identifier:
                case TokenType.OpenBrace:
                case TokenType.OpenInterpolation:
                    validStart = true;
                    break;
                case TokenType.Period:
                case TokenType.Hash:
                    var next = stream.Peek(1);
                    validStart = next.Type == TokenType.Identifier && next.Start == stream.Current.End;
                    break;
                // nested combinators and pseudo selectors
                case TokenType.Ampersand:
                case TokenType.GreaterThan:
                case TokenType.Plus:
                case TokenType.Tilde:
                case TokenType.Colon:
                case TokenType.DoubleColon:
                    validStart = parent is RuleBlock;
                    break;
            }

            if (validStart)
            {
                var next = stream.Peek(1);
                switch (next.Type)
                {
                    case TokenType.OpenCurlyBrace:
                    case TokenType.EndOfFile:
                    case TokenType.Comma:
                        return true;
                    case TokenType.OpenBrace:
                    case TokenType.GreaterThan:
                    case TokenType.Plus:
                    case TokenType.Tilde:
                        // allow combinators or attribute selector unless they stack
                        return stream.Current.Type != next.Type;
                    case TokenType.Colon:
                    case TokenType.DoubleColon:
                    case TokenType.Hash:
                    case TokenType.Period:
                    case TokenType.Identifier:
                    case TokenType.OpenInterpolation:
                        // identifier must be exactly after (#id or .class) or with whitespace (html body)
                        // psudeo selectors / elements are same basic situation
                        return next.Start >= stream.Current.End;
                    case TokenType.Ampersand:
                    default:
                        // anything else including whitespace
                        return next.Start > stream.Current.End;
                }
            }

            return false;
        }
    }
}
