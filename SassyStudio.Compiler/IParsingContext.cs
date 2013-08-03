using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler
{
    public interface IParsingExecutionContext
    {
        bool IsCancellationRequested { get; }
    }
}
