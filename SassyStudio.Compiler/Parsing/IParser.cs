using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public interface IParser
    {
        TimeSpan LastParsingDuration { get; }
        TimeSpan LastTokenizationDuration { get; }
        Task<ParseItemList> ParseAsync(ITextProvider text, IParsingExecutionContext context, ISassItemFactory itemFactory = null);
    }
}
