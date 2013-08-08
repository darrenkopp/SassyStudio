using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;

namespace SassyStudio.Intellisense
{
    abstract class CompletionAugmenterBase : ISassCompletionAugmenter
    {
        public virtual string Moniker { get { return null; } }
        public virtual string Name { get { return null; } }

        [Import]
        protected IGlyphService Glyphs { get; set; }

        public virtual IEnumerable<Completion> GetCompletions(SassCompletionContext context)
        {
            return null;
        }

        public virtual IEnumerable<Completion> GetBuilder(SassCompletionContext context)
        {
            return null;
        }

        protected Completion CreateKeyword(string name)
        {
            return new Completion(name, name, null, Glyphs.GetGlyph(StandardGlyphGroup.GlyphKeyword, StandardGlyphItem.TotalGlyphItems), null);
        }

        protected Completion CreateFunction(string name)
        {
            return new Completion(name, name, null, Glyphs.GetGlyph(StandardGlyphGroup.GlyphGroupMethod, StandardGlyphItem.GlyphItemPublic), null);
        }
    }
}
