using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Utilities;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Intellisense
{
    interface IVariableCompletionProvider : ISassCompletionProvider
    {
    }

    [Export(typeof(IVariableCompletionProvider))]
    class VariableCompletionProvider : CompletionProviderBase, IVariableCompletionProvider
    {
        protected override IEnumerable<string> GetCompletionItems(SassCompletionContext context)
        {
            foreach (var container in context.TraversalPath.OfType<IVariableScope>())
            {
                foreach (var variable in container.GetDefinedVariables(context.StartPosition).Where(x => x.IsValid && x.Start < context.StartPosition))
                    yield return variable.GetName(context.Text);
            }
        }
    }
}
