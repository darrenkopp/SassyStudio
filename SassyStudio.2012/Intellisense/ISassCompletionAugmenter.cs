using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;

namespace SassyStudio.Intellisense
{
    interface ISassCompletionAugmenter
    {
        string Moniker { get; }
        string Name { get; }
        IEnumerable<Completion> GetCompletions(SassCompletionContext context);
        IEnumerable<Completion> GetBuilder(SassCompletionContext context);
        //void Augment(SassCompletionContext context, IList<CompletionSet> completionSets);
    }
}
