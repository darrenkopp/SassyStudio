using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Lexing
{
    interface ILexerFactory
    {
        ILexer Create();
    }
}
