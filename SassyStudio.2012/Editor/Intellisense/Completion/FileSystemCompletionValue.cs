using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    class FileSystemCompletionValue : ICompletionValue
    {
        public FileSystemCompletionValue(string displayText, string completionText, bool closeValue = true)
        {
            DisplayText = displayText;
            CompletionText = completionText;
            if (closeValue)
                CompletionText += "\"";
        }

        public SassCompletionValueType Type { get { return SassCompletionValueType.Default; } }

        public string DisplayText { get; private set; }

        public string CompletionText { get; private set; }

        public string Description { get; private set; }
    }
}
