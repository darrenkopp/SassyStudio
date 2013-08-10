using SassyStudio.Compiler.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public abstract class BangModifier : ComplexItem
    {
        public BangModifier()
        {
            ClassifierType = SassClassifierType.ImportanceModifier;
        }

        public TokenItem Bang { get; protected set; }
        public TokenItem Flag { get; protected set; }
        protected abstract string FlagName { get; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Bang && IsValidModifier(text, stream.Peek(1), FlagName))
            {
                Bang = Children.AddCurrentAndAdvance(stream);
                Flag = Children.AddCurrentAndAdvance(stream);
            }

            return Children.Count > 0;
        }

        public static bool IsValidModifier(ITextProvider text, Token token, string name)
        {
            if (token.Type == TokenType.Identifier)
                return text.CompareOrdinal(token.Start, name);

            return false;
        }
    }
}
