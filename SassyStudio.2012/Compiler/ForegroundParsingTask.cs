using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler
{
    [Export(typeof(IForegroundParsingTask))]
    class ForegroundParsingTask : IForegroundParsingTask
    {
        [Import]
        IDocumentParserFactory ParserFactory { get; set; }

        public ISassStylesheet Parse(IParsingRequest request)
        {
            try
            {
                var parser = ParserFactory.Create();
                return parser.Parse(request);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Failed to parse document changes.");
                return null;
            }
        }
    }
}
