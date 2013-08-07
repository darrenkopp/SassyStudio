using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;
using SassyStudio.Compiler.Parsing;
using SassyStudio.Scss;

namespace SassyStudio.Intellisense
{
    [Export(typeof(ICompletionProvider))]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    class VariableCompletionProvider : ICompletionProvider
    {
        readonly Lazy<ImageSource> Icon;
        public VariableCompletionProvider()
        {
            Icon = new Lazy<ImageSource>(() => Glyphs.GetGlyph(StandardGlyphGroup.GlyphGroupVariable, StandardGlyphItem.GlyphItemPublic), true);
        }

        [Import]
        private IGlyphService Glyphs { get; set; }

        public IEnumerable<Completion> GetCompletions(ITextProvider text, IEnumerable<ComplexItem> containers, int end)
        {
            var visited = new HashSet<string>();
            foreach (var variable in containers.OfType<IVariableScope>().SelectMany(scope => scope.GetDefinedVariables(end)).Where(x => x.IsValid))
            {
                var name = variable.Name.GetName(text);
                if (visited.Add(name))
                    yield return new Completion(name, name, null, Icon.Value, null);
            }
        }
    }
}
