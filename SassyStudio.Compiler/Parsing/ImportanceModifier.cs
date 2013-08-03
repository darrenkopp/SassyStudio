using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SassyStudio.Compiler.Lexing;

namespace SassyStudio.Compiler.Parsing
{
    public class ImportanceModifier : ComplexItem
    {
        public TokenItem Bang { get; protected set; }
        public TokenItem ModifierName { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (IsImportanceModifier(text, stream))
            {
                Bang = Children.AddCurrentAndAdvance(stream, SassClassifierType.ImportanceModifier);
                ModifierName = Children.AddCurrentAndAdvance(stream, SassClassifierType.ImportanceModifier);
            }

            return Children.Count > 0;
        }

        public static bool IsImportanceModifier(ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Bang)
            {
                var name = stream.Peek(1);
                if (name.Type == TokenType.Identifier && (text.CompareOrdinal(name.Start, "important") || text.CompareOrdinal(name.Start, "default")))
                    return true;
            }

            return false;
        }
    }
}
