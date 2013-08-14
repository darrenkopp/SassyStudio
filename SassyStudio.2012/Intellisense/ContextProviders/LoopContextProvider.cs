using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Intellisense
{
    [Export(typeof(ICompletionContextProvider))]
    class LoopContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(SassCompletionContext context)
        {
            if (context.Current is EachLoopDirective)
            {
                var directive = context.Current as EachLoopDirective;
                if (directive.Rule != null && BeforeSuccessor(context.StartPosition, directive.Body))
                    yield return SassCompletionContextType.EachDirectiveListValue;
            }
            else if (context.Current is ForLoopDirective)
            {
                var directive = context.Current as ForLoopDirective;
                if (directive.Rule != null)
                {
                    if (directive.Variable == null || BeforeSuccessor(context.StartPosition, directive.FromKeyword, directive.ToKeyword ?? directive.ThroughKeyword, directive.Body))
                    {
                        yield return SassCompletionContextType.ForLoopVariable;
                    }
                    else if (directive.FromKeyword == null || BeforeSuccessor(context.StartPosition, directive.ToKeyword ?? directive.ThroughKeyword, directive.Body))
                    {
                        yield return SassCompletionContextType.ForLoopFromKeyword;
                    }
                    else if ((directive.ThroughKeyword == null || directive.FromKeyword == null) || BeforeSuccessor(context.StartPosition, directive.Body))
                    {
                        yield return SassCompletionContextType.ForLoopRangeKeyword;
                    }
                }
            }
            else if (context.Current is WhileLoopDirective)
            {
                var directive = context.Current as WhileLoopDirective;
                if (directive.Rule != null && BeforeSuccessor(context.StartPosition, directive.Body))
                    yield return SassCompletionContextType.WhileLoopCondition;
            }
            else
            {
                yield return SassCompletionContextType.EachDirective;
                yield return SassCompletionContextType.ForLoopDirective;
                yield return SassCompletionContextType.WhileLoopDirective;
            }
        }

        private bool BeforeSuccessor(int position, params ParseItem[] successors)
        {
            for (int i = 0; i < successors.Length; i++)
            {
                var successor = successors[i];
                if (successor != null && position >= successor.Start)
                    return false;
            }

            return true;
        }
    }
}
