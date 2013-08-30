using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class MediaQueryDirective : ComplexItem
    {
        readonly List<MediaQuery> _Queries = new List<MediaQuery>(0);
        public AtRule Rule { get; protected set; }
        public MediaQueryBlock Body { get; protected set; }
        public IReadOnlyCollection<MediaQuery> Queries { get { return _Queries; } }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, "media"))
            {
                Rule = AtRule.CreateParsed(itemFactory, text, stream);
                if (Rule != null)
                    Children.Add(Rule);

                while (!IsTerminator(stream.Current.Type))
                {
                    var query = itemFactory.CreateSpecific<MediaQuery>(this, text, stream);
                    if (query.Parse(itemFactory, text, stream))
                    {
                        _Queries.Add(query);
                        Children.Add(query);
                    }
                    else
                    {
                        Children.AddCurrentAndAdvance(stream);
                    }
                }

                var block = itemFactory.CreateSpecific<MediaQueryBlock>(this, text, stream);
                if (block.Parse(itemFactory, text, stream))
                {
                    Body = block;
                    Children.Add(block);
                }
            }

            return Children.Count > 0;
        }

        static bool IsTerminator(TokenType type)
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
