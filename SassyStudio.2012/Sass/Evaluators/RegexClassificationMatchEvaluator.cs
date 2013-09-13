using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Classification;

namespace SassyStudio.Sass.Classifications
{
    class RegexClassificationMatchEvaluator : IClassificationMatchEvaluator
    {
        readonly IClassificationType Type;
        readonly Regex Engine;
        public RegexClassificationMatchEvaluator(IClassificationType type, string pattern, RegexOptions options = RegexOptions.Singleline)
        {
            Type = type;
            Engine = new Regex(pattern, RegexOptions.Compiled | options);
        }

        public IEnumerable<ClassificationMatchResult> Matches(string text)
        {
            var match = Engine.Match(text);
            while (match.Success)
            {
                yield return ClassificationMatchResult.Create(Type, match.Index, match.Length);
                match = Engine.Match(text, match.Index + match.Length);
            }
        }
    }
}