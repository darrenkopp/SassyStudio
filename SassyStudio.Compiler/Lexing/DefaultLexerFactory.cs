using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Lexing
{
    [Export(typeof(ILexerFactory))]
    class DefaultLexerFactory : ILexerFactory
    {
        public ILexer Create()
        {
            return new Lexer();
        }
    }
}
