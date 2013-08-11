using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing.Selectors
{
    public class AttributeSelector : SimpleSelector
    {
        public TokenItem OpenBrace { get; protected set; }
        public TokenItem Attribute { get; protected set; }
        public TokenItem Operator { get; protected set; }
        public TokenItem Value { get; protected set; }
        public TokenItem CloseBrace { get; protected set; }
        protected override bool ParseSelectorToken(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.OpenBrace)
            {
                OpenBrace = Children.AddCurrentAndAdvance(stream, SassClassifierType.SquareBrace);
                if (stream.Current.Type == TokenType.Identifier)
                    Attribute = Children.AddCurrentAndAdvance(stream);

                if (IsAttributeOperator(stream.Current.Type))
                    Operator = Children.AddCurrentAndAdvance(stream);

                if (stream.Current.Type == TokenType.String || stream.Current.Type == TokenType.BadString)
                    Value = Children.AddCurrentAndAdvance(stream);

                if (stream.Current.Type == TokenType.CloseBrace)
                    CloseBrace = Children.AddCurrentAndAdvance(stream, SassClassifierType.SquareBrace);
            }

            return Children.Count > 0;
        }

        static bool IsAttributeOperator(TokenType type)
        {
            switch (type)
            {
                case TokenType.Equal:
                case TokenType.SubstringMatch:
                case TokenType.SuffixMatch:
                case TokenType.DashMatch:
                case TokenType.IncludeMatch:
                case TokenType.PrefixMatch:
                    return true;
                default:
                    return false;
            }
        }
    }
}
