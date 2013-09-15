using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    class MixinCompletionValue : ICompletionValue
    {
        public MixinCompletionValue(string name)
        {
            DisplayText = name;
            CompletionText = name;
        }

        public SassCompletionValueType Type { get { return SassCompletionValueType.Mixin; } }

        public string DisplayText { get; private set; }

        public string CompletionText { get; private set; }

        public string Description { get; private set; }
    }
}
