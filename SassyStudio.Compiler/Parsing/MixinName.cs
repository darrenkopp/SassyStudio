using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SassyStudio.Compiler.Lexing;

namespace SassyStudio.Compiler.Parsing
{
    public class MixinName : SimplexItem, IEquatable<MixinName>
    {
        public MixinName(SassClassifierType type)
        {
            ClassifierType = type;
        }

        public TokenItem Name { get; protected set; }
        private string NameValue { get; set; }
        public override bool IsValid { get { return Name != null && Name.Length > 0; } }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Identifier || stream.Current.Type == TokenType.Function)
            {
                Name = Children.AddCurrentAndAdvance(stream, ClassifierType);
                NameValue = text.GetText(Name.Start, Name.Length);
            }

            return Children.Count > 0;
        }

        public string GetName(ITextProvider text)
        {
            if (IsValid)
                return text.GetText(Name.Start, Name.Length);

            return null;
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

        public bool Equals(MixinName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return NameValue == other.NameValue;
        }
    }
}
