using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    class ClassName : ComplexItem
    {
        public ClassName(SassClassifierType classifierType = SassClassifierType.ClassName)
        {
            ClassifierType = classifierType;
        }

        public TokenItem Dot { get; protected set; }
        public TokenItem Name { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (IsValidName(stream))
            {
                Dot = Children.AddCurrentAndAdvance(stream, ClassifierType);
                Name = Children.AddCurrentAndAdvance(stream, ClassifierType);
            }

            return Children.Count > 0;
        }

        public static bool IsValidName(ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Period)
                return stream.Peek(1).Type == TokenType.Identifier;

            return false;
        }
    }
}
