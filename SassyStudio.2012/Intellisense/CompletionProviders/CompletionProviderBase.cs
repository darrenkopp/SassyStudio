using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;

namespace SassyStudio.Intellisense
{
    abstract class CompletionProviderBase : ISassCompletionProvider
    {
        public CompletionProviderBase()
        {
        }

        [Import]
        protected IGlyphService Glyphs { get; set; }

        protected virtual StandardGlyphGroup Group { get { return StandardGlyphGroup.GlyphGroupVariable; } }
        protected virtual StandardGlyphItem Item { get { return StandardGlyphItem.GlyphItemPublic; } }
        protected abstract IEnumerable<string> GetCompletionItems(SassCompletionContext context);

        public IEnumerable<Completion> GetCompletions(SassCompletionContext context)
        {
            var set = new HashSet<string>();
            foreach (var item in GetCompletionItems(context).Where(x => set.Add(x)))
                yield return new Completion(item, item, null, Glyphs.GetGlyph(Group, Item), null);
        }
    }
}
