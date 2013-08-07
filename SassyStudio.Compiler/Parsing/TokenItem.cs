using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SassyStudio.Compiler.Lexing;

namespace SassyStudio.Compiler.Parsing
{
    public class TokenItem : ParseItem
    {
        private int _Start;
        private int _End;
        private int _Length;
        
        public TokenItem(SassClassifierType classifierType = SassClassifierType.Default)
        {
            ClassifierType = classifierType;
        }

        public TokenItem(Token token, SassClassifierType classifierType = SassClassifierType.Default)
            : this(classifierType)
        {
            _Start = token.Start;
            _Length = token.Length;
            _End = token.Start + token.Length;
            SourceType = token.Type;
        }

        public override int Start { get { return _Start; } }
        public override int End { get { return _End; } }
        public override int Length { get { return _Length; } }
        public TokenType SourceType { get; protected set; }


        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type != TokenType.EndOfFile)
            {
                var token = stream.Current;
                _Start = token.Start;
                _Length = token.Length;
                _End = _Start + _Length;
                SourceType = token.Type;

                stream.Advance();
                return true;
            }

            return false;
        }
    }
}
