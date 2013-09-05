using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    class BlockScopeContainer : CompletionContainerBase
    {
        readonly BlockItem Item;
        public BlockScopeContainer(BlockItem item)
        {
            Item = item;
            Start = item.Start;
            End = item.End;
        }
    }
}
