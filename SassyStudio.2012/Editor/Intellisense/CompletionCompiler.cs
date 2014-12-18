using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    interface ICompletionCompiler
    {
        CompletionSet Compile(ITrackingSpan span, IEnumerable<ICompletionValue> values);
    }

    [Export(typeof(ICompletionCompiler))]
    class CompletionCompiler : ICompletionCompiler
    {
        readonly IDictionary<SassCompletionValueType, ImageSource> IconMappings;
        readonly IGlyphService Glyphs;

        [ImportingConstructor]
        public CompletionCompiler(IGlyphService glyphs)
        {
            Glyphs = glyphs;

            IconMappings = new Dictionary<SassCompletionValueType, ImageSource>
            {
                { SassCompletionValueType.Default, Glyphs.GetGlyph(StandardGlyphGroup.GlyphGroupUnknown, StandardGlyphItem.TotalGlyphItems) },
                { SassCompletionValueType.Keyword, Glyphs.GetGlyph(StandardGlyphGroup.GlyphKeyword, StandardGlyphItem.TotalGlyphItems) },
                { SassCompletionValueType.SystemFunction, Glyphs.GetGlyph(StandardGlyphGroup.GlyphGroupMethod, StandardGlyphItem.GlyphItemPublic) },
                { SassCompletionValueType.UserFunction, Glyphs.GetGlyph(StandardGlyphGroup.GlyphExtensionMethod, StandardGlyphItem.GlyphItemInternal) },
                { SassCompletionValueType.Mixin, Glyphs.GetGlyph(StandardGlyphGroup.GlyphGroupInterface, StandardGlyphItem.GlyphItemInternal) },
                { SassCompletionValueType.Variable, Glyphs.GetGlyph(StandardGlyphGroup.GlyphGroupVariable, StandardGlyphItem.GlyphItemInternal) },
                { SassCompletionValueType.Property, Glyphs.GetGlyph(StandardGlyphGroup.GlyphGroupProperty, StandardGlyphItem.TotalGlyphItems) }
            };
        }

        public CompletionSet Compile(ITrackingSpan span, IEnumerable<ICompletionValue> values)
        {
            return new CompletionSet(
                moniker: "Sass",
                displayName: "Sass",
                applicableTo: span,
                completions: new LinkedList<Completion>(Transform(values)).OrderBy(x => x.DisplayText, StringComparer.OrdinalIgnoreCase),
                completionBuilders: null
            );
        }

        private IEnumerable<Completion> Transform(IEnumerable<ICompletionValue> values)
        {
            var observed = new HashSet<string>();

            foreach (var value in values.Where(x => observed.Add(x.DisplayText)))
                yield return new Completion(
                    displayText: value.DisplayText,
                    insertionText: value.CompletionText,
                    description: value.Description,
                    iconSource: IconMappings[value.Type],
                    iconAutomationText: null
                );
        }
    }
}
