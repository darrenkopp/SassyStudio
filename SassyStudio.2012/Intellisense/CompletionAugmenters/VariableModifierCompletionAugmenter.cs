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
    [Export(typeof(ISassCompletionProvider))]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    class VariableModifierCompletionAugmenter : CompletionAugmenterBase
    {
        public override IEnumerable<Completion> GetBuilder(SassCompletionContext context)
        {
            if (context.Current is VariableDefinition && !(context.Current.Parent is FunctionArgumentDefinition))
            {
                var definition = context.Current as VariableDefinition;
                if (definition.Colon != null && definition.Modifier == null && definition.Values.Count > 0)
                    yield return CreateKeyword("!default");
            }

            yield break;
        }
    }
}
