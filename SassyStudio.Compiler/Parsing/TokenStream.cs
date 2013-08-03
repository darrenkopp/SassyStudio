using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SassyStudio.Compiler.Lexing;

namespace SassyStudio.Compiler.Parsing
{
    class TokenStream : ITokenStream
    {
        readonly IParsingExecutionContext Context;
        readonly TokenList Tokens;
        readonly Token END_OF_FILE_TOKEN;

        public TokenStream(TokenList tokens, IParsingExecutionContext context)
        {
            Context = context;
            Tokens = tokens ?? new TokenList();
            if (Tokens.Count == 0)
                Tokens.Add(Token.CreateEmpty(TokenType.EndOfFile, 0));

            END_OF_FILE_TOKEN = Tokens[Tokens.Count - 1];
            CachedIndex = int.MinValue;
        }

        private int CachedIndex { get; set; }
        private Token CachedToken { get; set; }

        public int Length { get { return Tokens.Count; } }
        public int Position { get; private set; }

        public Token Current
        {
            get
            {
                if (Context.IsCancellationRequested)
                    return END_OF_FILE_TOKEN;

                // update cache if different
                if (Position != CachedIndex)
                {
                    CachedIndex = Position;
                    CachedToken = Peek(0);
                }

                return CachedToken;
            }
        }

        public Token Peek(int offset)
        {
            // always return end of file when cancelled
            if (Context.IsCancellationRequested) 
                return END_OF_FILE_TOKEN;

            var index = Position + offset;
            if (index < 0)
                index = 0;

            if (index >= Tokens.Count)
                index = Tokens.Count - 1;

            return Tokens[index];
        }

        public Token Advance(int offset)
        {
            Position = Position + offset;
            return Current;
        }

        public Token Reverse(int offset)
        {
            Position = Position - offset;
            return Current;
        }

        public Token SeekTo(int position)
        {
            Position = position;
            return Current;
        }
    }
}
