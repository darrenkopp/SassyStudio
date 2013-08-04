using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class BlockItem : ComplexItem
    {
        public TokenItem OpenCurlyBrace { get; protected set; }
        public TokenItem CloseCurlyBrace { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.OpenCurlyBrace)
            {
                OpenCurlyBrace = Children.AddCurrentAndAdvance(stream, SassClassifierType.CurlyBrace);

                ParseBody(itemFactory, text, stream);

                if (stream.Current.Type == TokenType.CloseCurlyBrace)
                    CloseCurlyBrace = Children.AddCurrentAndAdvance(stream, SassClassifierType.CurlyBrace);
            }

            return Children.Count > 0;
        }

        protected virtual void ParseBody(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            while (!IsTerminator(stream.Current.Type))
            {
                ParseItem item;
                if (itemFactory.TryCreateParsedOrDefault(this, text, stream, out item))
                    Children.Add(item);
            }
        }

        protected static bool IsTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.CloseCurlyBrace:
                    return true;
            }

            return false;
        }
    }
}
