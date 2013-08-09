using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;
using Microsoft.VisualStudio.Language.Intellisense;

namespace SassyStudio.Intellisense
{
    interface IMixinCompletionProvider : ISassCompletionProvider
    {
    }

    //[Export(typeof(IMixinCompletionProvider))]
    class MixinCompletionProvider : CompletionProviderBase, IMixinCompletionProvider
    {
        protected override StandardGlyphGroup Group { get { return StandardGlyphGroup.GlyphGroupMethod; } }
        protected override StandardGlyphItem Item { get { return StandardGlyphItem.GlyphItemPublic; } }
        protected override IEnumerable<string> GetCompletionItems(SassCompletionContext context)
        {
            return context.TraversalPath
                .SelectMany(x => x.Children)
                .Where(x => x.Start < context.StartPosition)
                .OfType<MixinDefinition>()
                .Where(x => x.IsValid)
                .Select(mixin => context.Text.GetText(mixin.Name.Start, mixin.Name.Length));
        }
    }
}
