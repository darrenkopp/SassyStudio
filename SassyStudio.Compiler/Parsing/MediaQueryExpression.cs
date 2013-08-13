using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class MediaQueryExpression : ComplexItem
    {
        public MediaQueryExpression()
        {
            FeatureValues = new ParseItemList();
        }

        public TokenItem Combinator { get; protected set; }
        public TokenItem OpenBrace { get; protected set; }
        public ParseItem Feature { get; protected set; }
        public TokenItem Colon { get; protected set; }
        public ParseItemList FeatureValues { get; protected set; }
        public TokenItem CloseBrace { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Identifier && text.CompareOrdinal(stream.Current.Start, "and"))
                Combinator = Children.AddCurrentAndAdvance(stream, SassClassifierType.Keyword);

            if (stream.Current.Type == TokenType.OpenFunctionBrace)
                OpenBrace = Children.AddCurrentAndAdvance(stream, SassClassifierType.FunctionBrace);

            ParseItem feature;
            if (itemFactory.TryCreateParsedOrDefault(this, text, stream, out feature))
            {
                Feature = feature;
                Children.Add(feature);
            }

            if (stream.Current.Type == TokenType.Colon)
                Colon = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);

            // dump all values
            while (!IsExpressionTerminator(stream.Current.Type))
            {
                ParseItem value;
                if (itemFactory.TryCreateParsedOrDefault(this, text, stream, out value))
                {
                    FeatureValues.Add(value);
                    Children.Add(value);
                }
                else
                {
                    Children.AddCurrentAndAdvance(stream);
                }
            }

            if (stream.Current.Type == TokenType.CloseFunctionBrace)
                CloseBrace = Children.AddCurrentAndAdvance(stream, SassClassifierType.FunctionBrace);

            return Children.Count > 0;
        }

        private bool IsExpressionTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.CloseFunctionBrace:
                case TokenType.OpenCurlyBrace:
                    return true;
                default:
                    return false;
            }
        }
    }
}
