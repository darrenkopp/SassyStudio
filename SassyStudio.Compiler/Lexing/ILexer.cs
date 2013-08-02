using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Lexing
{
    interface ILexer
    {
        Task<TokenList> TokenizeAsync(ITextStream stream);
    }
}
