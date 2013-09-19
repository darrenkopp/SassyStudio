using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    class PropertyNameCompletionValue : ICompletionValue
    {
        public PropertyNameCompletionValue(string name, string description)
        {
            DisplayText = name;
            Description = description;
        }

        public string DisplayText { get; private set; }

        public string CompletionText { get { return DisplayText + ":"; } }

        public string Description { get; private set; }

        public SassCompletionValueType Type { get { return SassCompletionValueType.Property; } }
    }
}
