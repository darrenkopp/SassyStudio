using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    class StylesheetContainer : CompletionContainerBase
    {
        public override void Add(ParseItem item, ITextProvider text)
        {
            if (item is MixinDefinition)
            {
                var definition = item as MixinDefinition;
                Parse(new MixinContainer(definition), definition.Children, text);
            }
            else if (item is UserFunctionDefinition)
            {
                var definition = item as UserFunctionDefinition;
                Parse(new FunctionContainer(definition), definition.Children, text);
            }
            else if (item is BlockItem)
            {
            }
            else
            {
                base.Add(item, text);
            }
        }
    }
}
