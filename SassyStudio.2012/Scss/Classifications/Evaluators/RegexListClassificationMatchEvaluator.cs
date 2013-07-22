using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Classification;

namespace SassyStudio.Scss.Classifications
{
    class RegexListClassificationMatchEvaluator : IClassificationMatchEvaluator
    {
        readonly ICollection<string> Candidates;
        readonly IClassificationType Type;
        readonly Regex Engine;
        public RegexListClassificationMatchEvaluator(IClassificationType type, ICollection<string> candidates, string pattern, RegexOptions options = RegexOptions.Singleline)
        {
            Candidates = candidates;
            Type = type;
            Engine = new Regex(pattern, RegexOptions.Compiled | RegexOptions.ExplicitCapture | options);
        }

        public IEnumerable<ClassificationMatchResult> Matches(string text)
        {
            var match = Engine.Match(text);
            while (match.Success)
            {
                Group value = match.Groups["value"];
                if (value.Success && Candidates.Contains(value.Value))
                    yield return ClassificationMatchResult.Create(Type, value.Index, value.Length);

                match = Engine.Match(text, match.Index + match.Length);
            }
        }
    }
}
