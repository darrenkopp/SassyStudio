using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SassyStudio.Compiler.Lexing;

namespace SassyStudio.Compiler.Parsing
{
    [Export(typeof(IParser))]
    class Parser : IParser
    {
        private readonly ILexer Lexer;
        public Parser(ILexer lexer)
        {
            Lexer = lexer;
        }

        public TimeSpan LastParsingDuration { get; protected set; }
        public TimeSpan LastTokenizationDuration { get; protected set; }

        public async Task<ParseItemList> ParseAsync(ITextProvider text, IParsingExecutionContext context, ISassItemFactory itemFactory)
        {
            var tokens = await TokenizeAsync(text, context);

            var watch = Stopwatch.StartNew();

            var stream = CreateTokenStream(tokens, context);
            var results = Parse(text, new ItemFactory(itemFactory), stream, context);

            watch.Stop();
            LastParsingDuration = watch.Elapsed;
            return results;
        }

        private ParseItemList Parse(ITextProvider text, IItemFactory itemFactory, ITokenStream stream, IParsingExecutionContext context)
        {
            var results = new ParseItemList();
            while (!context.IsCancellationRequested && stream.Current.Type != TokenType.EndOfFile)
            {
                int position = stream.Position;

                ParseItem item;
                if (!itemFactory.TryCreate(null, text, stream, out item))
                    break;

                if (item.Parse(itemFactory, text, stream))
                    results.Add(item);

                // guard against infinite loop (in case token couldn't be handled)
                if (stream.Position == position)
                    stream.Advance();
            }

            // freeze everything
            if (!context.IsCancellationRequested)
                foreach (var item in results)
                    item.Freeze();

            return results;
        }

        private async Task<TokenList> TokenizeAsync(ITextProvider text, IParsingExecutionContext context)
        {
            var stream = new TextStream(text);
            var tokens = await Lexer.TokenizeAsync(stream, context);
            LastTokenizationDuration = Lexer.LastTokenizationDuration;

            return tokens;
        }

        protected virtual ITokenStream CreateTokenStream(TokenList tokens, IParsingExecutionContext context)
        {
            return new TokenStream(tokens, context);
        }
    }
}
