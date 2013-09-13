using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace SassyStudio.Sass.Classifications
{
    interface IClassificationMatchEvaluator
    {
        IEnumerable<ClassificationMatchResult> Matches(string text);
    }
}
