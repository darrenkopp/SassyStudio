using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class UrlItem : ComplexItem
    {
        public TokenItem Function { get; set; }
        public TokenItem OpenBrace { get; set; }
        public StringValue Url { get; set; }
        public TokenItem CloseBrace { get; set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (IsUrl(text, stream.Current))
            {
                Function = Children.AddCurrentAndAdvance(stream, SassClassifierType.SystemFunction);

                if (stream.Current.Type == TokenType.OpenFunctionBrace)
                    OpenBrace = Children.AddCurrentAndAdvance(stream, SassClassifierType.FunctionBrace);

                if ((stream.Current.Type == TokenType.String || stream.Current.Type == TokenType.BadString))
                {
                    Url = itemFactory.CreateSpecificParsed<StringValue>(this, text, stream);
                    if (Url != null)
                        Children.Add(Url);
                }
                else
                {
                    // not using string, so just consume everything until close of url()
                    while (!IsUrlTerminator(stream.Current.Type))
                    {
                        Children.AddCurrentAndAdvance(stream, SassClassifierType.String);
                    }
                }

                if (stream.Current.Type == TokenType.CloseFunctionBrace)
                    CloseBrace = Children.AddCurrentAndAdvance(stream, SassClassifierType.FunctionBrace);
            }

            return Children.Count > 0;
        }

        public static bool IsUrl(ITextProvider text, Token token)
        {
            switch (token.Type)
            {
                case TokenType.Function:
                case TokenType.Identifier:
                    return text.StartsWithOrdinal(token.Start, "url");
                default:
                    return false;
            }
        }

        static bool IsUrlTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.CloseFunctionBrace:
                case TokenType.NewLine:
                case TokenType.EndOfFile:
                case TokenType.Semicolon:
                    return true;
                default:
                    return false;
            }
        }
    }
}
