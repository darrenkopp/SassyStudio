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
    class ValueCompletionAugmenter : CompletionAugmenterBase
    {
        [Import]
        IVariableCompletionProvider VariableProvider { get; set; }

        [Import]
        IFunctionCompletionProvider FunctionProvider { get; set; }

        public override IEnumerable<Completion> GetBuilder(SassCompletionContext context)
        {
            if (IsValueContext(context.Current) || IsValueContext(context.Current.Parent))
            {
                bool valid = false;
                var property = context.Current as PropertyDeclaration;
                if (property != null && property.Colon != null)
                    valid = true;

                var variable = context.Current as VariableDefinition;
                if (variable != null && variable.Colon != null)
                    valid = true;

                var function = context.Current as FunctionArgumentDefinition;
                if (function != null && function.Variable != null && function.Variable.Colon != null)
                    valid = true;

                if (valid)
                    return VariableProvider.GetCompletions(context)
                        .Concat(FunctionProvider.GetCompletions(context));
            }

            return base.GetBuilder(context);
        }

        private bool IsValueContext(ParseItem current)
        {
            return current is VariableDefinition || current is FunctionArgumentDefinition || current is PropertyDeclaration;
        }
    }
}
