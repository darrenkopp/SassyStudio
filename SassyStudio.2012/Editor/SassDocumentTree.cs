using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor
{
    class SassDocumentTree : ISassDocumentTree
    {
        readonly ITextSnapshot _SourceText;
        readonly ParseItemList _Items;
        public SassDocumentTree(ITextSnapshot source, ParseItemList items)
        {
            _SourceText = source;
            _Items = items;
        }

        public ITextSnapshot SourceText { get { return _SourceText; } }

        public ParseItemList Items { get { return _Items; } }
    }
}
