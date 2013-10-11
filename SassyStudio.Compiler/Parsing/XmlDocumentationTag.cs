using System.Collections.Generic;

namespace SassyStudio.Compiler.Parsing
{
    public class XmlDocumentationTag : ComplexItem
    {
        readonly List<XmlAttribute> _Attributes = new List<XmlAttribute>(0); 

        public TokenItem OpenTag { get; protected set; }
        public TokenItem Name { get; protected set; }
        public IReadOnlyList<XmlAttribute> Attributes { get; protected set; }
        public TokenItem CloseSlash { get; protected set; }
        public TokenItem CloseTag { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.LessThan)
            {
                OpenTag = Children.AddCurrentAndAdvance(stream, SassClassifierType.XmlDocumentationTag);
                if (stream.Current.Type == TokenType.Identifier)
                {
                    Name = Children.AddCurrentAndAdvance(stream, SassClassifierType.XmlDocumentationTag);

                    while (!IsTagTerminator(stream.Current.Type))
                    {
                        var attribute = itemFactory.CreateSpecific<XmlAttribute>(this, text, stream);
                        if (!attribute.Parse(itemFactory, text, stream))
                            break;

                        Children.Add(attribute);
                        _Attributes.Add(attribute);
                    }
                }

                if (stream.Current.Type == TokenType.Slash)
                    CloseSlash = Children.AddCurrentAndAdvance(stream, SassClassifierType.XmlDocumentationTag);

                if (stream.Current.Type == TokenType.GreaterThan)
                    CloseTag = Children.AddCurrentAndAdvance(stream, SassClassifierType.XmlDocumentationTag);
            }

            OutputLogger.Log("Woot, parsed a doc tag");
            return Children.Count > 0;
        }

        public override void Freeze()
        {
            _Attributes.TrimExcess();
            base.Freeze();
        }

        static bool IsTagTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.Slash:
                case TokenType.NewLine:
                    return true;
                default:
                    return false;
            }
        }
    }
}
