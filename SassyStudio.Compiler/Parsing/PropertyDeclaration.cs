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
        public PropertyDeclaration()
        {
            Values = new ParseItemList();
        }

        public PropertyName Name { get; protected set; }
        public TokenItem Colon { get; protected set; }
        public ImportanceModifier Modifier { get; protected set; }
        public TokenItem Semicolon { get; protected set; }
        public ParseItemList Values { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (IsDeclaration(stream))
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
                case TokenType.Semicolon:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsDeclaration(ITokenStream stream)
        {
            int position = stream.Position;

            bool validPropertyName = false;
            while (true)
            {
                var last = stream.Current;
                var next = stream.Advance();

                if (next.Start > last.End || IsDeclrationTerminator(last.Type))
                    break;

                if (next.Type == TokenType.Colon)
                {
                    var value = stream.Peek(1);
                    switch (value.Type)
                    {
                        case TokenType.Identifier:
                        case TokenType.Function:
                            validPropertyName = value.Start > next.End;
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

        private static bool IsDeclrationTerminator(TokenType type)
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
    }
}
