using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    class PseudoCompletionValue : ICompletionValue
    {
        public PseudoCompletionValue(string name, string description)
        {
            DisplayText = CompletionText = name;
            Description = description;
        }

        public SassCompletionValueType Type { get { return SassCompletionValueType.Default; } }

        public string DisplayText { get; private set; }

        public string CompletionText { get; private set; }

        public string Description { get; private set; }
    }
}
