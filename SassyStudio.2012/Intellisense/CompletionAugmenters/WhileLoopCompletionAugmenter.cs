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
    class WhileLoopCompletionAugmenter : CompletionAugmenterBase
    {
        readonly Lazy<IEnumerable<Completion>> Keywords;
        public WhileLoopCompletionAugmenter()
        {
            Keywords = new Lazy<IEnumerable<Completion>>(() => new[] { CreateKeyword("@while") }, true);
        }

        [Import]
        protected IVariableCompletionProvider VariableProvider { get; set; }

        public override IEnumerable<Completion> GetBuilder(SassCompletionContext context)
        {
            if (context.Current is Stylesheet || context.Current is BlockItem)
            {
                return Keywords.Value;
            }
            else
            {
                var directive = context.Current as WhileLoopDirective;
                if (directive != null && directive.Rule != null && (directive.Body == null || context.StartPosition < directive.Body.Start))
                    return VariableProvider.GetCompletions(context);
            }

            return base.GetBuilder(context);
        }
    }
}
