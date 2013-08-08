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
    [Export(typeof(ISassCompletionAugmenter))]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    class ConditionalCompletionAugmenter : CompletionAugmenterBase
    {
        readonly Lazy<IEnumerable<Completion>> Keywords;
        public ConditionalCompletionAugmenter()
        {
            Keywords = new Lazy<IEnumerable<Completion>>(() => new[] { CreateKeyword("@if"), CreateKeyword("@else"), CreateKeyword("@else if") }, true);
        }

        [Import]
        IVariableCompletionProvider VariableProvider { get; set; }

        [Import]
        IFunctionCompletionProvider FunctionProvider { get; set; }

        public override IEnumerable<Completion> GetBuilder(SassCompletionContext context)
        {
            if (context.Current is Stylesheet || context.Current is BlockItem)
            {
                return Keywords.Value;
            }
            else if (context.Current is ConditionalControlDirective)
            {
                var directive = context.Current as ConditionalControlDirective;
                if (directive.Rule != null && context.Text.GetText(directive.Rule.Start, directive.Rule.Length) != "@else")
                {
                    return VariableProvider.GetCompletions(context)
                        .Concat(FunctionProvider.GetCompletions(context));
                }
            }

            return base.GetBuilder(context);
        }
    }
}
