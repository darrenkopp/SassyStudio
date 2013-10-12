using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Lexing;

namespace SassyStudio.Compiler.Parsing
{
    class DocumentParser : IDocumentParser
    {
        readonly ILexer Lexer;
        readonly IDocumentManager DocumentManager;

        public DocumentParser(ILexer lexer, IDocumentManager documentManager)
        {
            Lexer = lexer;
            DocumentManager = documentManager;
        }

        public ISassStylesheet Parse(IParsingRequest request)
        {
            var tokens = Tokenize(request);
            var stream = CreateTokenStream(tokens, new ParsingRequestExecutionContext(request));
            var itemFactory = new ItemFactory(new DefaultSassItemFactory());

            var stylesheet = new Stylesheet();
            if (stylesheet.Parse(itemFactory, request.Text, stream) && !request.IsCancelled)
            {
                stylesheet.Freeze();

                foreach (var import in stylesheet.Children.OfType<SassImportDirective>())
                    import.ResolveImports(request.Text, request.Document, DocumentManager);

                foreach (var reference in ResolveReferences(stylesheet))
                    reference.ResolveImports(request.Text, request.Document, DocumentManager);

                return stylesheet;
            }

            return null;
        }

        private TokenList Tokenize(IParsingRequest request)
        {
            var stream = new TextStream(request.Text);

            return Lexer.Tokenize(stream, new ParsingRequestLexingContext(request));
        }

        protected virtual ITokenStream CreateTokenStream(TokenList tokens, IParsingExecutionContext context)
        {
            return new TokenStream(tokens, context);
        }

        IEnumerable<FileReferenceTag> ResolveReferences(Stylesheet stylesheet)
        {
            return stylesheet
                .Children.OfType<XmlDocumentationComment>()
                .SelectMany(x => x.Children.OfType<FileReferenceTag>());
        }

        class ParsingRequestLexingContext : ILexingContext
        {
            readonly IParsingRequest Request;

            public ParsingRequestLexingContext(IParsingRequest request)
            {
                Request = request;
            }

            public bool IsCancellationRequested { get { return Request.IsCancelled; } }
        }

        class ParsingRequestExecutionContext : IParsingExecutionContext
        {
            readonly IParsingRequest Request;

            public ParsingRequestExecutionContext(IParsingRequest request)
            {
                Request = request;
            }

            public bool IsCancellationRequested { get { return Request.IsCancelled; } }
        }
    }
}
