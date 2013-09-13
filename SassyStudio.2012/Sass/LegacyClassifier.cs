using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using SassyStudio.Scss;
using SassyStudio.Scss.Classifications;

namespace SassyStudio.Sass.Classifications
{
    class LegacyClassifier : IClassifier
    {
        readonly IClassificationMatchEvaluator[] MatchEvaluators;
        public LegacyClassifier(IClassificationTypeRegistryService registry)
        {
            MatchEvaluators = GetEvaluators(registry).ToArray();
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var text = span.GetText();
            var results = new List<ClassificationSpan>();
            foreach (var evaluator in MatchEvaluators)
            {
                var matches = evaluator.Matches(text);

                results.AddRange(matches
                    .Select(x => CreateSpan(span, x.Type, x.Index, x.Length))
                    // exlude any / all results already contained by existing results
                    .Where(x => !results.Any(r => r.Span.Contains(x.Span)))
                    .ToList()
                );
            }

            return results;
        }

        private static ClassificationSpan CreateSpan(SnapshotSpan span, IClassificationType type, int index, int length)
        {
            return new ClassificationSpan(new SnapshotSpan(span.Snapshot, span.Start + index, length), type);
        }

        IEnumerable<IClassificationMatchEvaluator> GetEvaluators(IClassificationTypeRegistryService registry)
        {
            var keywords = registry.GetClassificationType(ScssClassificationTypes.Keyword);
            var mixin_references = registry.GetClassificationType(ScssClassificationTypes.MixinReference);

            // strings are highest in priority since they can contain anything really
            var strings = registry.GetClassificationType(ScssClassificationTypes.StringValue);
            yield return new RegexClassificationMatchEvaluator(strings, @"([""'])(?:(?=(\\?))\2.)*?\1");

            // comments
            var comments = registry.GetClassificationType(ScssClassificationTypes.Comment);
            yield return new RegexClassificationMatchEvaluator(comments, @"//.*");

            // parent references
            var parent_references = registry.GetClassificationType(ScssClassificationTypes.ParentReference);
            yield return new RegexClassificationMatchEvaluator(parent_references, @"&");

            // @mixin definitions
            var mixin_definitions = registry.GetClassificationType(ScssClassificationTypes.MixinDefinition);
            yield return RegexGroupClassificationMatchEvaluator.Create(
                @"(?<keyword>@mixin) (?<mixin>[^\(\{\s]+)",
                Tuple.Create("keyword", keywords),
                Tuple.Create("mixin", mixin_definitions)
            );

            // @function definitions
            var user_function_definitions = registry.GetClassificationType(ScssClassificationTypes.UserFunctionDefinition);
            yield return RegexGroupClassificationMatchEvaluator.Create(
                @"(?<keyword>@function) (?<name>[^\(\{\s]+)",
                Tuple.Create("keyword", keywords),
                Tuple.Create("name", user_function_definitions)
            );

            // @keywords
            yield return new RegexClassificationMatchEvaluator(
                keywords,
                @"@(charset|font-face|media|page|import|debug|warn|if|else|else if|for|while|return|extend)"
            );

            // @include something (mixin reference)
            yield return RegexGroupClassificationMatchEvaluator.Create(
                @"(?<keyword>@include) (?<mixin>[^\(;]+)",
                Tuple.Create("keyword", keywords),
                Tuple.Create("mixin", mixin_references)
            );

            // !important or !default
            var modifiers = registry.GetClassificationType(ScssClassificationTypes.ImportanceModifier);
            yield return new RegexClassificationMatchEvaluator(modifiers, @"\![ \t]*(important|default)");

            // variables
            var variable_definitions = registry.GetClassificationType(ScssClassificationTypes.VariableDefinition);
            var variable_references = registry.GetClassificationType(ScssClassificationTypes.VariableReference);
            yield return new RegexClassificationMatchEvaluator(variable_definitions, @"(?<name>(\!|\$)\w[a-zA-Z0-9-_]*)(?=:)", RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
            yield return new RegexClassificationMatchEvaluator(variable_references, @"(?<name>(\!|\$)\w[a-zA-Z0-9-_]*)(?!:)", RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

            // function references
            yield return new RegexListClassificationMatchEvaluator(
                registry.GetClassificationType(ScssClassificationTypes.FunctionReference),
                new HashSet<string>(ScssWellKnownFunctionNames.Names, StringComparer.Ordinal),
                @"(?<value>[a-z-]+)\("
            );

            // interpolation support
            var interpolation = registry.GetClassificationType(ScssClassificationTypes.Interpolation);
            yield return RegexGroupClassificationMatchEvaluator.Create(
                @"(?<start>#\{)(?<body>[^\}]+)(?<end>\})",
                Tuple.Create("start", interpolation),
                Tuple.Create("body", variable_references),
                Tuple.Create("end", interpolation)
            );
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged { add { } remove { } }
    }
}
