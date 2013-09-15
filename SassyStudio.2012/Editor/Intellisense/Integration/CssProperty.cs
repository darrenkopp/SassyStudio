using System;
using System.Collections.Generic;

namespace SassyStudio.Editor.Intellisense
{
    class CssProperty : CssSchemaItem
    {
        readonly IDictionary<string, CssPropertyValue> _Values = new Dictionary<string, CssPropertyValue>(StringComparer.Ordinal);
        public CssProperty(string name, string description)
            : base(name, description)
        {
        }

        public void AddValue(CssPropertyValue value)
        {
            if (!_Values.ContainsKey(value.Name))
                _Values.Add(value.Name, value);
        }
    }

    class CssPropertyValue : CssSchemaItem
    {
        public CssPropertyValue(string name, string description)
            : base(name, description)
        {
        }
    }
}