using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace SassyStudio.Intellisense
{
    interface ICompletionManager : IDisposable
    {
        CompletionSet CreateCompletionSetFor(ITrackingSpan span, ICompletionSession session);
    }
}
