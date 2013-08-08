using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Intellisense
{
    [Export(typeof(ISassCompletionAugmenter))]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    class ForLoopCompletionAugmenter : CompletionAugmenterBase
    {
        public override IEnumerable<Completion> GetCompletions(SassCompletionContext context)
        {
            var completions = new LinkedList<Completion>();
            if (context.Current is ForLoopDirective)
            {
                var directive = context.Current as ForLoopDirective;
                if (directive.Variable != null && directive.FromKeyword == null)
                    completions.AddLast(CreateKeyword("from"));

                if (directive.FromKeyword != null && directive.ThroughKeyword == null && directive.ToKeyword == null)
                {
                    completions.AddLast(CreateKeyword("through"));
                    completions.AddLast(CreateKeyword("to"));
                }
            }
            else
            {
                completions.AddLast(CreateKeyword("@for"));
            }

            return completions;
        }
    }
}
