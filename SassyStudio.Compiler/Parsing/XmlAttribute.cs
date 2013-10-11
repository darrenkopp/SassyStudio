using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class XmlAttribute : ComplexItem
    {
        public TokenItem Name { get; protected set; }
        public TokenItem EqualSign { get; protected set; }
        public StringValue Value { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Identifier)
            {
                Name = Children.AddCurrentAndAdvance(stream, SassClassifierType.XmlDocumentationTag);

                if (stream.Current.Type == TokenType.Equal)
                    EqualSign = Children.AddCurrentAndAdvance(stream);

                if (stream.Current.Type == TokenType.String || stream.Current.Type == TokenType.BadString)
                    Value = itemFactory.CreateSpecificParsed<StringValue>(this, text, stream);
            }

            return Children.Count > 0;
        }
    }
}
