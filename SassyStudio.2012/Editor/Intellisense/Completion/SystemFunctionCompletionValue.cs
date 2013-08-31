using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    class SystemFunctionCompletionValue : ICompletionValue
    {
        public SassCompletionValueType Type { get { return SassCompletionValueType.SystemFunction; } }

        public string DisplayText { get; set; }

        public string CompletionText { get; set; }

        public int Start { get; set; }

        public int End { get; set; }

        public int Length { get; set; }
    }
}
