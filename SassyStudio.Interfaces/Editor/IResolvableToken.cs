using System;
using System.Collections.Generic;
using System.Linq;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor
{
    public interface IResolvableToken
    {
        ParseItem GetSourceToken();
    }
}
