using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Lexing
{
    interface ILexer
    {
        TimeSpan LastTokenizationDuration { get; }
        TokenList Tokenize(ITextStream text, ILexingContext context);
        Task<TokenList> TokenizeAsync(ITextStream stream, IParsingExecutionContext context);
    }

    interface ILexingContext
    {
        bool IsCancellationRequested { get; }
    }
}
