using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    class UserFunctionCompletionValue : ICompletionValue
    {
        public UserFunctionCompletionValue(string name)
        {
            DisplayText = name;
            CompletionText = name;
        }

        public SassCompletionValueType Type { get { return SassCompletionValueType.UserFunction; } }

        public string DisplayText { get; private set; }

        public string CompletionText { get; private set; }

        public string Description { get; private set; }
    }
}
