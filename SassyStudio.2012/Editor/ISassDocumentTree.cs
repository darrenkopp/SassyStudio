using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor
{
    public interface ISassDocumentTree
    {
        ITextSnapshot SourceText { get; }
        ParseItemList Items { get; }
    }
}
