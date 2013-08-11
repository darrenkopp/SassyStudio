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
            if (PropertyName.IsValidName(stream))
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
            int start = stream.Position;
            bool valid = false;
            if (PropertyName.IsValidName(stream))
            {
                var last = stream.Current;
                while (!IsValueTerminator(stream.Advance().Type))
                {
                    // have to be sequential tokens for valid property name
                    if (stream.Current.Start != last.End)
                        break;

                    if (stream.Current.Type == TokenType.Colon)
                    {
                        valid = true;
                        break;
                    }

                    last = stream.Current;
                }
            }

            stream.SeekTo(start);
            return valid;
        }
    }
}
