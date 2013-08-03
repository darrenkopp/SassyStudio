using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler
{
    public class ParsingExecutionContext : IParsingExecutionContext
    {
        private readonly IParsingCancellationToken CancellationToken;
        public ParsingExecutionContext(IParsingCancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }

        public bool IsCancellationRequested { get { return CancellationToken.IsCancellationRequested; } }
    }
}
