using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Classification;

namespace SassyStudio.Sass.Classifications
{
    class RegexGroupClassificationMatchEvaluator : IClassificationMatchEvaluator
    {
        readonly Regex Engine;
        readonly Tuple<string, IClassificationType>[] GroupMappings;
        public RegexGroupClassificationMatchEvaluator(Tuple<string, IClassificationType>[] mappings, string pattern, RegexOptions options = RegexOptions.Singleline | RegexOptions.ExplicitCapture)
        {
            GroupMappings = mappings;
            Engine = new Regex(pattern, RegexOptions.Compiled | options);
        }

        public IEnumerable<ClassificationMatchResult> Matches(string text)
        {
            var match = Engine.Match(text);
            while (match.Success)
            {
                for (int i = 0; i < GroupMappings.Length; i++)
                {
                    var mapping = GroupMappings[i];
                    var group = match.Groups[mapping.Item1];
                    if (group.Success)
                        yield return ClassificationMatchResult.Create(mapping.Item2, group.Index, group.Length);
                }

                match = Engine.Match(text, match.Index + match.Length);
            }
        }

        public static IClassificationMatchEvaluator Create(string pattern, params Tuple<string, IClassificationType>[] mappings)
        {
            return new RegexGroupClassificationMatchEvaluator(mappings, pattern);
        }
    }
}
