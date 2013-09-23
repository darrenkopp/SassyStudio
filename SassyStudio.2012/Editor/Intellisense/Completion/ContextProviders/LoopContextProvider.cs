using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionContextProvider))]
    class LoopContextProvider : ICompletionContextProvider
    {
        public IEnumerable<SassCompletionContextType> GetContext(ICompletionContext context)
        {
            var current = context.Current;
            var position = context.Position;
            if (current is EachLoopDirective)
            {
                var directive = current as EachLoopDirective;
                if (directive.Rule != null && BeforeSuccessor(position, directive.Body))
                    yield return SassCompletionContextType.EachDirectiveListValue;
            }
            else if (current is ForLoopDirective)
            {
                var directive = current as ForLoopDirective;
                if (directive.Rule != null)
                {
                    if (directive.Variable == null || BeforeSuccessor(position, directive.FromKeyword, directive.ToKeyword ?? directive.ThroughKeyword, directive.Body))
                    {
                        yield return SassCompletionContextType.ForLoopVariable;
                    }
                    else if (directive.FromKeyword == null || BeforeSuccessor(position, directive.ToKeyword ?? directive.ThroughKeyword, directive.Body))
                    {
                        yield return SassCompletionContextType.ForLoopFromKeyword;
                    }
                    else if ((directive.ThroughKeyword == null || directive.FromKeyword == null) || BeforeSuccessor(position, directive.Body))
                    {
                        yield return SassCompletionContextType.ForLoopRangeKeyword;
                    }
                }
            }
            else if (current is WhileLoopDirective)
            {
                var directive = current as WhileLoopDirective;
                if (directive.Rule != null && BeforeSuccessor(position, directive.Body))
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
