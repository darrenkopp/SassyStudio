using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    class PropertyCompletionValue : ICompletionValue
    {
        public PropertyCompletionValue(string name, string description)
        {
            DisplayText = CompletionText = name;
            Description = description;
        }

        public string DisplayText { get; private set; }

        public string CompletionText { get; private set; }

        public string Description { get; private set; }

        public SassCompletionValueType Type { get { return SassCompletionValueType.Property; } }
    }
}
