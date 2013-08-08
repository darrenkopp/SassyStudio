using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using SassyStudio.Compiler.Parsing;
using SassyStudio.Scss;

namespace SassyStudio.Intellisense
{
    interface IFunctionCompletionProvider : ISassCompletionProvider
    {
    }

    [Export(typeof(IFunctionCompletionProvider))]
    class FunctionCompletionProvider : CompletionProviderBase, IFunctionCompletionProvider
    {
        protected override StandardGlyphGroup Group { get { return StandardGlyphGroup.GlyphGroupMethod; } }
        protected override StandardGlyphItem Item { get { return StandardGlyphItem.GlyphItemPublic; } }

        protected override IEnumerable<string> GetCompletionItems(SassCompletionContext context)
        {
            var systemFunctions = ScssWellKnownFunctionNames.Names;
            var userFunctions = context.TraversalPath
                .SelectMany(x => x.Children)
                .Where(x => x.Start < context.StartPosition)
                .OfType<UserFunctionDefinition>()
                .Where(x => x.IsValid)
                .Select(function => context.Text.GetText(function.Name.Start, function.Name.Length));

            return systemFunctions.Concat(systemFunctions);
        }
    }
}
