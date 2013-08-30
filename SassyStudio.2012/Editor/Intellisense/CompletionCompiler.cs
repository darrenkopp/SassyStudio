using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    interface ICompletionCompiler
    {
        CompletionSet Compile(ISassDocument document, int position, ITrackingSpan span);
    }

    class CompletionCompiler : ICompletionCompiler
    {
        readonly IIntellisenseManager Manager;
        readonly IGlyphService Glyphs;

        [ImportingConstructor]
        public CompletionCompiler(IIntellisenseManager manager, IGlyphService glyphs)
        {
            Manager = manager;
            Glyphs = glyphs;
        }

        public CompletionSet Compile(ISassDocument document, int position, ITrackingSpan span)
        {
            var cache = Manager.Get(document);
            var context = CreateContext(document, position);

            return Compile(cache, context, span);
        }

        private CompletionSet Compile(IIntellisenseCache cache, ICompletionContext context, ITrackingSpan span)
        {
            var types = Manager.GetCompletionContextTypes(context);
            var set = new CompletionSet("SCSS", "SCSS", span, GetCompletions(context, types), null);
            
            return set;
        }

        private IEnumerable<Completion> GetCompletions(ICompletionContext context, IEnumerable<SassCompletionContextType> types)
        {
            var results = (
                from type in types
                from provider in Manager.GetCompletions(type)
                from value in provider.GetCompletions(type, context)
                select new Completion(
                    displayText: value.DisplayText, 
                    insertionText: value.CompletionText, 
                    description: null, 
                    iconSource: Glyphs.GetGlyph(StandardGlyphGroup.GlyphGroupField, StandardGlyphItem.GlyphItemPublic), 
                    iconAutomationText: null
                )
            );

            return results;
        }

        ICompletionContext CreateContext(ISassDocument document, int position)
        {
            var stylesheet = document.Stylesheet;
            if (stylesheet == null) return null;

            var item = stylesheet.Children.FindItemContainingPosition(position);
            while (item != null)
            {
                // if we have a complex item, stop searching
                if (item is ComplexItem)
                    break;

                item = item.Parent;
            }

            return new CompletionContext
            {
                Current = item ?? stylesheet as Stylesheet,
                Position = position
            };
        }

        class CompletionContext : ICompletionContext
        {
            public ParseItem Current { get; internal set; }

            public int Position { get; internal set; }
        }
    }
}
