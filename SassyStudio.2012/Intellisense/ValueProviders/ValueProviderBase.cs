using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;

namespace SassyStudio.Intellisense
{
    abstract class ValueProviderBase : ICompletionValueProvider
    {
        [Import]
        protected IGlyphService Glyphs { get; private set; }

        public abstract IEnumerable<SassCompletionContextType> SupportedContexts { get; }

        public abstract IEnumerable<Completion> GetCompletions(SassCompletionContextType type, SassCompletionContext context);

        protected Completion Keyword(string name)
        {
            return Create(name, name, StandardGlyphGroup.GlyphKeyword, StandardGlyphItem.TotalGlyphItems);
        }

        protected Completion Variable(string name)
        {
            return Create(name, name, StandardGlyphGroup.GlyphGroupField, StandardGlyphItem.GlyphItemPublic);
        }

        protected Completion Function(string name, StandardGlyphGroup group = StandardGlyphGroup.GlyphGroupMethod, StandardGlyphItem item = StandardGlyphItem.GlyphItemPublic)
        {
            return Create(name, name, group, item);
        }

        protected Completion Create(string display, string insertion, StandardGlyphGroup group, StandardGlyphItem item)
        {
            return new Completion(display, insertion, null, Glyphs.GetGlyph(group, item), null);
        }
    }
}
