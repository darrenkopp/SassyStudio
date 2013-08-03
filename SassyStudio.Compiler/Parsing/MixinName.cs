using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SassyStudio.Compiler.Lexing;

namespace SassyStudio.Compiler.Parsing
{
    class MixinName : ComplexItem
    {
        public MixinName(SassClassifierType type)
        {
            ClassifierType = type;
        }

        public TokenItem Name { get; protected set; }
        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Identifier || stream.Current.Type == TokenType.Function)
                Name = Children.AddCurrentAndAdvance(stream, ClassifierType);

            return Children.Count > 0;
        }

        public static MixinName CreateParsed(IItemFactory itemFactory, ITextProvider text, ITokenStream stream, SassClassifierType classifierType)
        {
            var name = new MixinName(classifierType);
            if (name.Parse(itemFactory, text, stream))
                return name;

            return null;
        }

        public static bool IsValidName(Token token)
        {
            return token.Type == TokenType.Identifier || token.Type == TokenType.Function;
        }
    }
}
