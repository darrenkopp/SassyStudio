using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class MediaQuery : ComplexItem
    {
        readonly List<MediaQueryExpression> _Expressions = new List<MediaQueryExpression>(0);

        public TokenItem Modifier { get; protected set; }
        public ParseItem MediaType { get; protected set; }
        public IReadOnlyCollection<MediaQueryExpression> Expressions { get { return _Expressions; } }
        public TokenItem Comma { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Identifier && IsModifier(text.GetText(stream.Current.Start, stream.Current.Length)))
                Modifier = Children.AddCurrentAndAdvance(stream);

            ParseItem mediaType;
            if (itemFactory.TryCreateParsedOrDefault(this, text, stream, out mediaType))
            {
                MediaType = mediaType;
                Children.Add(mediaType);
            }

            while (!IsTerminator(text, stream))
            {
                var expression = itemFactory.CreateSpecific<MediaQueryExpression>(this, text, stream);
                if (expression.Parse(itemFactory, text, stream))
                {
                    _Expressions.Add(expression);
                    Children.Add(expression);
                }
                else
                {
                    Children.AddCurrentAndAdvance(stream);
                }
            }

            if (stream.Current.Type == TokenType.Comma)
                Comma = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);

            return Children.Count > 0;
        }

        private bool IsTerminator(ITextProvider text, ITokenStream stream)
        {
            switch (stream.Current.Type)
            {
                case TokenType.EndOfFile:
                case TokenType.Comma:
                case TokenType.OpenCurlyBrace:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsModifier(string value)
        {
            switch (value)
            {
                case "only":
                case "not":
                    return true;
                default:
                    return false;
            }
        }
    }
}
