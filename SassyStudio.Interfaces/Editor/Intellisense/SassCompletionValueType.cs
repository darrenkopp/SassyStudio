using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    public enum SassCompletionValueType
    {
        Default = 0,
        Keyword,
        SystemFunction,
        UserFunction,
        Mixin,
        Variable,
        Property
    }
}
