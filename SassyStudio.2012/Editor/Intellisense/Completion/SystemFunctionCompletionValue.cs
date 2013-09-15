using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    class SystemFunctionCompletionValue : ICompletionValue
    {
        public SystemFunctionCompletionValue(string name)
        {
            CompletionText = DisplayText = name;
        }

        public SassCompletionValueType Type { get { return SassCompletionValueType.SystemFunction; } }

        public string DisplayText { get; private set; }

        public string CompletionText { get; private set; }

        public string Description { get; private set; }
    }
}
