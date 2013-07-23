using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SassyStudio.Scss.Taggers
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IErrorTag))]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    class DeprecatedFunctionalityTaggingProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer != null)
                return buffer.Properties.GetOrCreateSingletonProperty<DeprecatedFunctionalityTagger>(() => new DeprecatedFunctionalityTagger()) as ITagger<T>;

            return null;
        }
    }

    class DeprecatedFunctionalityTagger : ITagger<IErrorTag>
    {
        readonly Tuple<Regex, string>[] WarningProviders;
        public DeprecatedFunctionalityTagger()
        {
            WarningProviders = new[] {
                CreateProvider(@"\![\w\d-_]+(?<!(important|default))", "Variables in the form !name are no longer supported. Use the $name syntax instead."),
                CreateProvider(@"(?:(?:\!|\$)[\w\d-_]+)(?=\s+?=)", "Variable assignment using '=' is no longer supported. Use the $name: value syntax instead.")
            };
        }

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var checkedLines = new HashSet<int>();
            foreach (var span in spans)
            {
                var line = span.Start.GetContainingLine();
                if (checkedLines.Add(line.LineNumber))
                {
                    var position = span.Start.Position;
                    var text = line.GetTextIncludingLineBreak();

                    foreach (var warning in FindWarnings(span, text).Where(x => span.IntersectsWith(x.Span)))
                        yield return warning;
                }
            }
        }

        private IEnumerable<ITagSpan<IErrorTag>> FindWarnings(SnapshotSpan source, string text)
        {
            foreach (var provider in WarningProviders)
            {
                var regex = provider.Item1;
                var match = regex.Match(text);
                while (match.Success)
                {
                    var span = new SnapshotSpan(source.Snapshot, new Span(source.Start + match.Index, match.Length));
                    yield return new TagSpan<IErrorTag>(span, new ErrorTag("Warning", provider.Item2));

                    match = match.NextMatch();
                }
            }
        }

        private static Tuple<Regex,string> CreateProvider(string pattern, string message)
        {
            return Tuple.Create(new Regex(pattern, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), message);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged { add { } remove { } }
    }
}
