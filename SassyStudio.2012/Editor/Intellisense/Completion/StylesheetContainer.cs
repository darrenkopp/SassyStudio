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
        readonly IIntellisenseManager IntellisenseManager;
        public StylesheetContainer(IIntellisenseManager manager)
        {
            IntellisenseManager = manager;
        }

        public override void Add(ParseItem item, ITextProvider text)
        {
            if (item is SassImportDirective)
            {
                var directive = item as SassImportDirective;
                foreach (var file in directive.Files.Where(x => x.IsValid))
                    Containers.Add(new ImportContainer(directive, IntellisenseManager.Get(file.Document)));
            }
            else if (item is MixinDefinition)
            {
                var definition = item as MixinDefinition;
                Parse(new MixinContainer(definition), definition.Children, text);

                // TODO: add mixin name to this container
            }
            else if (item is UserFunctionDefinition)
            {
                var definition = item as UserFunctionDefinition;
                Parse(new FunctionContainer(definition), definition.Children, text);

                // TODO: add function name to this container
            }
            else
            {
                base.Add(item, text);
            }
        }
    }
}
