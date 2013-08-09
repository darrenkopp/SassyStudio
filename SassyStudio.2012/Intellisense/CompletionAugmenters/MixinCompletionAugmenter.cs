using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Intellisense
{
    //[Export(typeof(ISassCompletionAugmenter))]
    //[ContentType(ScssContentTypeDefinition.ScssContentType)]
    class MixinCompletionAugmenter : CompletionAugmenterBase
    {
        [Import]
        IMixinCompletionProvider MixinProvider { get; set; }

        public override IEnumerable<Completion> GetBuilder(SassCompletionContext context)
        {
            if (context.Current is MixinReference)
            {
                return MixinProvider.GetCompletions(context);
            }
            else if (context.Current is BlockItem)
            {
                return new[] { CreateKeyword("@include") };
            }
            else if (context.Current is Stylesheet)
            {
                return new[] { CreateKeyword("@mixin") };
            }

            return base.GetBuilder(context);
        }
    }
}
