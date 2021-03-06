﻿using System.Collections.Generic;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    public interface ICompletionContextProvider
    {
        IEnumerable<SassCompletionContextType> GetContext(ICompletionContext context);
    }
}
