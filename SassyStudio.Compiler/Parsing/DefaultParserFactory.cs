using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Lexing;

namespace SassyStudio.Compiler.Parsing
{
    [Export(typeof(IParserFactory))]
    class DefaultParserFactory : IParserFactory
    {
        [Import]
        protected ILexerFactory LexerFactory { get; set; }

        public IParser Create()
        {
            var lexer = LexerFactory.Create();
            return new Parser(lexer);
        }
    }
}
