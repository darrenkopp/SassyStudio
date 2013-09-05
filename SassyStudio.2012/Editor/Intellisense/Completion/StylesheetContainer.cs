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
                if (definition.Name != null && definition.Name.Name != null)
                {
                    Parse(new MixinContainer(definition), definition.Children, text);

                    var name = text.GetText(definition.Name.Name.Start, definition.Name.Name.Length);
                    _Mixins.Add(definition.End, new MixinCompletionValue(name));
                }
            }
            else if (item is UserFunctionDefinition)
            {
                var definition = item as UserFunctionDefinition;
                if (definition.Name != null)
                {
                    Parse(new FunctionContainer(definition), definition.Children, text);

                    var name = text.GetText(definition.Name.Start, definition.Name.Length);
                    _Functions.Add(definition.End, new UserFunctionCompletionValue(name));
                }
            }
            else
            {
                base.Add(item, text);
            }
        }
    }
}
