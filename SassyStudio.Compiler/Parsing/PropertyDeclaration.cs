using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Lexing;

namespace SassyStudio.Compiler.Parsing
{
    public class PropertyDeclaration : ComplexItem
    {
        static readonly ICollection<string> WellKnownPseudoSelectors = CreatePseudoSelectors();

        public PropertyDeclaration()
        {
            Values = new ParseItemList();
        }

        public PropertyName Name { get; protected set; }
        public TokenItem Colon { get; protected set; }
        public ImportanceModifier Modifier { get; protected set; }
        public TokenItem Semicolon { get; protected set; }
        public ParseItemList Values { get; protected set; }
        public override bool IsUnclosed { get { return Colon == null || (Colon != null && Semicolon == null); } }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            //if (IsDeclaration(text, stream))
            if (stream.Current.Type == TokenType.Identifier || stream.Current.Type == TokenType.OpenInterpolation)
            {
                var name = itemFactory.CreateSpecific<PropertyName>(this, text, stream);
                if (name.Parse(itemFactory, text, stream))
                {
                    Name = name;
                    Children.Add(name);
                }

                if (stream.Current.Type == TokenType.Colon)
                    Colon = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);

                while (!IsValueTerminator(stream.Current.Type))
                {
                    ParseItem value;
                    if (itemFactory.TryCreateParsedOrDefault(this, text, stream, out value))
                    {
                        Values.Add(value);
                        Children.Add(value);
                    }
                }

                // nested property block support
                if (stream.Current.Type == TokenType.OpenCurlyBrace)
                {
                    var block = itemFactory.CreateSpecific<NestedPropertyBlock>(this, text, stream);
                    if (block.Parse(itemFactory, text, stream))
                        Children.Add(block);
                }

                if (stream.Current.Type == TokenType.Bang)
                {
                    var modifier = new ImportanceModifier();
                    if (modifier.Parse(itemFactory, text, stream))
                    {
                        Modifier = modifier;
                        Children.Add(modifier);
                    }
                }

                if (stream.Current.Type == TokenType.Semicolon)
                    Semicolon = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);
            }

            return Children.Count > 0;
        }

        static bool IsValueTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.OpenCurlyBrace:
                case TokenType.CloseCurlyBrace:
                case TokenType.Semicolon:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsDeclaration(ITextProvider text, ITokenStream stream)
        {
            int position = stream.Position;

            bool validPropertyName = false;
            while (true)
            {
                var last = stream.Current;
                var next = stream.Advance();

                if (next.Start > last.End || IsDeclarationTerminator(last.Type))
                    break;

                if (next.Type == TokenType.Colon)
                {
                    var value = stream.Peek(1);
                    switch (value.Type)
                    {
                        case TokenType.Ampersand:
                            validPropertyName = false;
                            break;
                        case TokenType.Identifier:
                        case TokenType.Function:
                            validPropertyName = value.Start > next.End || !IsPseudoSelector(text, value);
                            break;
                        default:
                            validPropertyName = true;
                            break;
                    }
                    break;
                }
            }

            stream.SeekTo(position);
            return validPropertyName;
        }

        static bool IsPseudoSelector(ITextProvider text, Token token)
        {
            switch (token.Type)
            {
                case TokenType.Identifier:
                case TokenType.Function:
                    return WellKnownPseudoSelectors.Contains(text.GetText(token.Start, token.Length));
                default:
                    return true;
            }
        }

        private static bool IsDeclarationTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.Semicolon:
                case TokenType.Comma:
                case TokenType.OpenCurlyBrace:
                    return true;
                default:
                    return false;
            }
        }

        private static ICollection<string> CreatePseudoSelectors()
        {
            return new HashSet<string>(StringComparer.Ordinal)
            {
                "active",
                "after",
                "before",
                "checked",
                "default",
                "dir",
                "disabled",
                "empty",
                "enabled",
                "first",
                "first-child",
                "first-of-type",
                "focus",
                "fullscreen",
                "hover",
                "indeterminate",
                "in-range",
                "invalid",
                "lang",
                "last-child",
                "last-of-type",
                "left",
                "link",
                "not",
                "nth-child",
                "nth-last-child",
                "nth-last-of-type",
                "nth-of-type",
                "only-child",
                "only-of-type",
                "optional",
                "out-of-range",
                "read-only",
                "read-write",
                "required",
                "right",
                "root",
                "scope",
                "target",
                "valid",
                "visited"
            };
        }
    }
}
