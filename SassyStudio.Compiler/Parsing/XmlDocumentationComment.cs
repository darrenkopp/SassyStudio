using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class XmlDocumentationComment : ComplexItem
    {
        protected TokenItem Opening { get; private set; }
        public XmlDocumentationTag Tag { get; private set; }
        protected TokenItem NewLine { get; private set; }


        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.XmlDocumentationComment)
            {
                Opening = Children.AddCurrentAndAdvance(stream, SassClassifierType.XmlDocumentationTag);

                // look for <reference
                if (stream.Current.Type == TokenType.LessThan)
                {
                    var tag = itemFactory.CreateSpecific<XmlDocumentationTag>(this, text, stream);
                    if (tag.Parse(itemFactory, text, stream))
                    {
                        Tag = tag;
                        Children.Add(tag);
                    }
                    else
                    {
                        Children.AddCurrentAndAdvance(stream, SassClassifierType.XmlDocumentationComment);
                    }
                }
                else
                {
                    while (!IsCommentTerminator(stream.Current.Type))
                    {
                        Children.AddCurrentAndAdvance(stream, SassClassifierType.XmlDocumentationComment);
                    }
                }

                //if (stream.Current.Type == TokenType.NewLine)
                //    NewLine = Children.AddCurrentAndAdvance(stream);
            }

            return Children.Count > 0;
        }

        static bool IsCommentTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.NewLine:
                    return true;
                default:
                    return false;
            }
        }
    }
}
