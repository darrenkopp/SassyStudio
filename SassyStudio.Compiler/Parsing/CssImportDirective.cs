using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class CssImportDirective : ImportDirective
    {
        readonly List<MediaQuery> _MediaQueries = new List<MediaQuery>(0);

        public UrlItem Url { get; protected set; }
        public TokenItem Filename { get; protected set; }
        public IReadOnlyCollection<MediaQuery> MediaQueries { get { return _MediaQueries; } }

        protected override void ParseImport(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.String || stream.Current.Type == TokenType.BadString)
            {
                Filename = Children.AddCurrentAndAdvance(stream, SassClassifierType.String);
            }
            else if (UrlItem.IsUrl(text, stream.Current))
            {
                var url = new UrlItem();
                if (url.Parse(itemFactory, text, stream))
                {
                    Url = url;
                    Children.Add(url);
                }
            }

            while (!IsTerminator(stream.Current.Type))
            {
                var query = new MediaQuery();
                if (!query.Parse(itemFactory, text, stream))
                    break;

                _MediaQueries.Add(query);
                Children.Add(query);
            }
        }

        public override void Freeze()
        {
            _MediaQueries.TrimExcess();
            base.Freeze();
        }

        static bool IsTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.Semicolon:
                case TokenType.OpenCurlyBrace:
                case TokenType.CloseCurlyBrace:
                    return true;
                default:
                    return false;
            }
        }
    }
}
