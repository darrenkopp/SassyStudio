using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Lexing;

namespace SassyStudio.Compiler.Parsing
{
    [Export(typeof(IDocumentParserFactory))]
    class DocumentParserFactory : IDocumentParserFactory
    {
        [Import]
        ILexerFactory LexerFactory { get; set; }

        [Import]
        IDocumentManager DocumentManager { get; set; }

        public IDocumentParser Create()
        {
            var lexer = LexerFactory.Create();
            return new DocumentParser(lexer, DocumentManager);
        }
    }
}
