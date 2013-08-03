using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Lexing
{
    public class Lexer : ILexer
    {
        public TimeSpan LastTokenizationDuration { get; protected set; }
        TokenList Tokenize(ITextStream stream, IParsingExecutionContext context)
        {
            DateTime start = DateTime.Now;
            var tokens = new TokenList();
            tokens.Add(Token.CreateEmpty(TokenType.StartOfFile, stream.Position));

            while (!context.IsCancellationRequested)
            {
                if (stream.Position >= stream.Length)
                    break;

                if (ConsumeComment(stream, tokens))
                    continue;

                if (ConsumeWhitespace(stream))
                    continue;

                Token token;
                if (TryCreateToken(stream, out token))
                    tokens.Add(token);
            }

            // close stream with end of file token
            tokens.Add(Token.CreateEmpty(TokenType.EndOfFile, stream.Length));

            LastTokenizationDuration = DateTime.Now - start;
            return tokens;
        }

        public Task<TokenList> TokenizeAsync(ITextStream stream, IParsingExecutionContext context)
        {
            return Task.Run(() => Tokenize(stream, context));
        }

        private bool ConsumeComment(ITextStream stream, TokenList tokens)
        {
            int start = stream.Position;
            if (stream.Current == '/')
            {
                var next = stream.Peek(1);
                if (next == '/')
                {
                    stream.Advance(2);
                    tokens.Add(Token.Create(TokenType.CppComment, start, 2));

                    ConsumeCommentText(stream, tokens, s => IsNewLine(s.Peek(1)));

                    return true;
                }
                else if (next == '*')
                {
                    stream.Advance(2);
                    tokens.Add(Token.Create(TokenType.OpenCssComment, start, 2));

                    ConsumeCommentText(stream, tokens, s => s.Peek(1) == '*');

                    if (stream.Current == '*' && stream.Peek(1) == '/')
                    {
                        tokens.Add(Token.Create(TokenType.CloseCssComment, stream.Position, 2));
                        stream.Advance(2);
                    }

                    return true;
                }

                return false;
            }

            return false;
        }

        private bool ConsumeCommentText(ITextStream stream, TokenList tokens, Func<ITextStream, bool> predicate)
        {
            int start = stream.Position;
            while (stream.Position < stream.Length)
            {
                if (predicate(stream))
                    break;

                stream.Advance();
            }

            if (start != stream.Position)
            {
                stream.Advance();
                tokens.Add(Token.Create(TokenType.CommentText, start, stream.Position - start));
                return true;
            }

            return false;
        }

        private bool TryCreateToken(ITextStream stream, out Token token)
        {
            token = default(Token);
            if (stream.Position >= stream.Length) return false;

            int start = stream.Position;
            TokenType type = TokenType.Unknown;
            switch (stream.Current)
            {
                // whitespace characters
                //case ' ':
                //case '\r':
                //case '\f':
                //case '\n':
                //case '\t':
                //    type = TokenType.Whitespace;
                //    ConsumeWhitespace(stream);
                //    break;
                // quoted strings
                case '\'':
                case '"':
                    ConsumeString(stream, out type);
                    break;
                // hash
                case '#':
                    type = ConsumeHash(stream);
                    break;
                case '$':
                    type = ConsumeDollar(stream);
                    break;
                case '(':
                    type = TokenType.OpenFunctionBrace;
                    stream.Advance();
                    break;
                case ')':
                    type = TokenType.CloseFunctionBrace;
                    stream.Advance();
                    break;
                case '*':
                    type = ConsumeAsterisk(stream);
                    break;
                case '+':
                    type = ConsumePlus(stream);
                    break;
                case ',':
                    type = TokenType.Comma;
                    stream.Advance();
                    break;
                case '-':
                    type = HandleHyphen(stream);
                    break;
                case '.':
                    type = ConsumePeriod(stream);
                    break;
                case '/':
                    type = TokenType.Slash;
                    stream.Advance();
                    break;
                case ':':
                    type = TokenType.Colon;
                    stream.Advance();
                    break;
                case ';':
                    type = TokenType.Semicolon;
                    stream.Advance();
                    break;
                case '<':
                    type = HandleLessThanSign(stream);
                    break;
                case '[':
                    type = TokenType.OpenBrace;
                    stream.Advance();
                    break;
                case ']':
                    type = TokenType.CloseBrace;
                    stream.Advance();
                    break;
                case '{':
                    type = TokenType.OpenCurlyBrace;
                    stream.Advance();
                    break;
                case '}':
                    type = TokenType.CloseCurlyBrace;
                    stream.Advance();
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    type = TokenType.Number;
                    ConsumeNumber(stream);
                    break;
                //case 'u':
                //case 'U':
                //    type = ConsumeUnicode(stream);
                //    break;
                case '|':
                    type = ConsumePipe(stream);
                    break;
                case '~':
                    type = ConsumeTilde(stream);
                    break;
                default:
                    // just add as unknown token
                    type = TokenType.Unknown;
                    stream.Advance();
                    break;
            }

            if (type == TokenType.Unknown && ConsumeIdentifier(stream))
                type = TokenType.Identifier;

            token = Token.Create(type, start, stream.Position - start);
            return true;
        }

        private TokenType ConsumeUnicode(ITextStream stream)
        {
            // TODO: attempt to figure out if unicode range
            return TokenType.Unknown;
        }

        private TokenType ConsumeTilde(ITextStream stream)
        {
            stream.Advance();
            if (stream.Current == '=')
            {
                stream.Advance();
                return TokenType.IncludeMatch;
            }

            return TokenType.Tilde;
        }

        private TokenType ConsumePipe(ITextStream stream)
        {
            stream.Advance();
            if (stream.Current == '=')
            {
                stream.Advance();
                return TokenType.DashMatch;
            }

            if (stream.Current == '|')
            {
                stream.Advance();
                return TokenType.Column;
            }

            return TokenType.Pipe;
        }

        private TokenType HandleLessThanSign(ITextStream stream)
        {
            stream.Advance();
            if (stream.Peek(1) == '!' && stream.Peek(2) == '-' && stream.Peek(3) == '-')
            {
                stream.Advance(3);
                return TokenType.OpenHtmlComment;
            }

            return TokenType.LessThanSign;
        }

        private TokenType ConsumePeriod(ITextStream stream)
        {
            if (ConsumeNumber(stream))
                return TokenType.Number;

            stream.Advance();
            return TokenType.Period;
        }

        private TokenType HandleHyphen(ITextStream stream)
        {
            stream.Advance();
            if (ConsumeNumber(stream))
                return TokenType.Number;

            stream.Reverse();
            if (ConsumeIdentifier(stream))
                return TokenType.Identifier;

            stream.Advance();
            if (stream.Peek(1) == '-' && stream.Peek(2) == '>')
            {
                stream.Advance(2);
                return TokenType.CloseHtmlComment;
            }

            return TokenType.Minus;
        }

        private bool ConsumeIdentifier(ITextStream stream)
        {
            return stream.InUndoContext(start =>
            {
                if (IsNameStart(stream.Current))
                {
                    stream.Advance();
                    ConsumeNameCharacters(stream);

                    return true;
                }

                if (stream.Current == '-')
                {
                    if (IsNameStart(stream.Peek(1)))
                    {
                        stream.Advance();
                        ConsumeNameCharacters(stream);
                        return true;
                    }

                    if (IsValidEscape(stream.Peek(1), stream.Peek(2)))
                    {
                        stream.Advance(2);
                        ConsumeNameCharacters(stream);
                        return true;
                    }
                }

                if (IsValidEscape(stream.Current, stream.Peek(1)))
                {
                    stream.Advance(2);
                    ConsumeNameCharacters(stream);
                    return true;
                }

                return false;
            });
        }

        void ConsumeNameCharacters(ITextStream stream)
        {
            while (IsValidNameCharacter(stream.Current))
                stream.Advance();
        }

        private bool IsValidNameCharacter(char c)
        {
            return IsNameStart(c) || char.IsDigit(c) || c == '-';
        }

        private bool IsNameStart(char c)
        {
            return char.IsLetter(c) || c == '_' || IsNonAscii(c);
        }

        private bool IsNonAscii(char c)
        {
            return (int)c >= 80;
        }

        private TokenType ConsumePlus(ITextStream stream)
        {
            var type = TokenType.Plus;
            stream.Advance();

            if (ConsumeNumber(stream))
                type = TokenType.Number;

            return type;
        }

        private bool ConsumeNumber(ITextStream stream)
        {
            bool checkDecimal = !ConsumeDecimalPart(stream);
            if (char.IsDigit(stream.Current))
            {
                while (char.IsDigit(stream.Current))
                    stream.Advance();

                if (stream.Current == '.' && ConsumeDecimalPart(stream))
                {
                    while (char.IsDigit(stream.Current))
                        stream.Advance();
                }

                return true;
            }

            return false;
        }

        private bool ConsumeDecimalPart(ITextStream stream)
        {
            if (stream.Current == '.' && char.IsDigit(stream.Peek(1)))
            {
                stream.Advance();
                return true;
            }

            return false;
        }

        private TokenType ConsumeAsterisk(ITextStream stream)
        {
            var type = TokenType.Asterisk;
            stream.Advance();
            if (stream.Current == '=')
            {
                type = TokenType.SubstringMatch;
                stream.Advance();
            }

            return type;
        }

        private TokenType ConsumeDollar(ITextStream stream)
        {
            var type = TokenType.Dollar;
            stream.Advance();

            if (stream.Current == '=')
            {
                type = TokenType.SuffixMatch;
                stream.Advance();
            }

            return type;
        }

        private TokenType ConsumeHash(ITextStream stream)
        {
            var type = TokenType.Hash;
            stream.Advance();
            if (stream.Current == '{')
            {
                type = TokenType.StringInterpolation;

                // consume until close paren
                while (stream.Advance())
                {
                    if (stream.Current == '}')
                    {
                        // consume } as well
                        stream.Advance();
                        break;
                    }
                }
            }

            return type;
        }

        private bool ConsumeString(ITextStream stream, out TokenType type)
        {
            type = TokenType.Unknown;
            switch (stream.Current)
            {
                case '\'':
                case '"':
                    {
                        var open = stream.Current;
                        while (stream.Advance())
                        {
                            // check for valid escapes
                            if (stream.Current == '\\')
                            {
                                stream.Advance();
                                if (IsNewLine(stream.Current))
                                {
                                    stream.Advance();
                                    continue;
                                }

                                // if escaping open quote, consume and advance
                                if (stream.Current == open)
                                {
                                    stream.Advance();
                                    continue;
                                }

                                if (IsValidEscape('\\', stream.Current))
                                {
                                    stream.Advance();
                                    continue;
                                }
                            }

                            // unescaped new line is bad news bears
                            if (IsNewLine(stream.Current))
                            {
                                // go back to right before the new line
                                stream.Reverse(1);
                                type = TokenType.BadString;
                                return true;
                            }

                            // happy days, we have properly quoted string
                            if (stream.Current == open)
                                break;
                        }

                        // consume closing quote
                        if (stream.Current == open)
                            stream.Advance();

                        type = TokenType.String;
                        break;
                    }
            }

            return type != TokenType.Unknown;
        }

        private bool IsValidEscape(char first, char second)
        {
            if (first == '\\')
            {
                if (IsNewLine(second))
                    return false;

                return true;
            }

            return false;
        }

        private bool ConsumeWhitespace(ITextStream stream)
        {
            switch (stream.Current)
            {
                case ' ':
                case '\t':
                case '\f':
                case '\n':
                case '\r':
                    while (IsWhitespace(stream.Current))
                        stream.Advance();

                    return true;
            }

            return false;
        }

        static bool IsWhitespace(char c)
        {
            switch (c)
            {
                case ' ':
                case '\t':
                    return true;
                default:
                    return IsNewLine(c);
            }
        }

        static bool IsNewLine(char c)
        {
            switch (c)
            {
                case '\n':
                case '\r':
                case '\f':
                    return true;
                default:
                    return false;
            }
        }
    }
}
