using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class ConditionalControlDirective : ControlDirective
    {
        readonly List<ConditionalControlDirective> _ElseStatements = new List<ConditionalControlDirective>(0);
        public ConditionalControlDirective()
        {
            ConditionStatements = new ParseItemList();
        }

        public ParseItemList ConditionStatements { get; protected set; }
        public ICollection<ConditionalControlDirective> ElseStatements { get { return _ElseStatements; } }
        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if ((IsConditionalDirective(text, stream) || IsConditionalContinuationDirective(text, stream)))
            {
                ParseRule(itemFactory, text, stream);

                while (!IsConditionTerminator(stream.Current.Type))
                {
                    ParseItem item;
                    if (itemFactory.TryCreateParsedOrDefault(this, text, stream, out item))
                    {
                        Children.Add(item);
                        ConditionStatements.Add(item);
                    }
                    else
                    {
                        Children.AddCurrentAndAdvance(stream);
                    }
                }

                ParseBody(itemFactory, text, stream);

                while (IsConditionalContinuationDirective(text, stream))
                {
                    var subsequentConditional = itemFactory.CreateSpecific<ConditionalControlDirective>(this, text, stream);
                    if (!subsequentConditional.Parse(itemFactory, text, stream))
                        break;

                    ElseStatements.Add(subsequentConditional);
                    Children.Add(subsequentConditional);
                }
            }

            return Children.Count > 0;
        }

        public override void Freeze()
        {
            base.Freeze();
            ConditionStatements.TrimExcess();
            _ElseStatements.TrimExcess();
        }

        public static bool IsConditionalDirective(ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.At)
            {
                var name = stream.Peek(1);
                if (name.Type == TokenType.Identifier)
                    return text.CompareOrdinal(name.Start, "if");
            }

            return false;
        }

        public static bool IsConditionalContinuationDirective(ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.At)
            {
                var name = stream.Peek(1);
                if (name.Type == TokenType.Identifier)
                    return text.StartsWithOrdinal(name.Start, "else");
            }

            return false;
        }

        private bool IsConditionTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.OpenCurlyBrace:
                    return true;

                default:
                    return false;
            }
        }
    }
}
