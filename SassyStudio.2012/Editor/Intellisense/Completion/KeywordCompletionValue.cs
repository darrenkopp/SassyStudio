using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    class KeywordCompletionValue : ICompletionValue
    {
        public KeywordCompletionValue(string name)
        {
            DisplayText = name;
            CompletionText = name;
        }

        public SassCompletionValueType Type { get { return SassCompletionValueType.Keyword; } }

        public string DisplayText { get; private set; }

        public string CompletionText { get; private set; }
    }
}
