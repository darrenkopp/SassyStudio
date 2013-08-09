using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;

namespace SassyStudio.Intellisense
{
    [Export(typeof(ICompletionValueProvider))]
    class KeywordsProvider : ValueProviderBase
    {
        public override IEnumerable<SassCompletionContextType> SupportedContexts
        {
            get
            {
                yield return SassCompletionContextType.AtDirectiveName;
                yield return SassCompletionContextType.ConditionalDirective;
                yield return SassCompletionContextType.CssDirective;
                yield return SassCompletionContextType.EachDirective;
                yield return SassCompletionContextType.ExtendDirective;
                yield return SassCompletionContextType.ExtendDirectiveOptionalFlag;
                yield return SassCompletionContextType.ForLoopDirective;
                yield return SassCompletionContextType.ForLoopFromKeyword;
                yield return SassCompletionContextType.ForLoopRangeKeyword;
                yield return SassCompletionContextType.FunctionBody;
                yield return SassCompletionContextType.FunctionDirective;
                yield return SassCompletionContextType.ImportDirective;
                yield return SassCompletionContextType.IncludeDirective;
                yield return SassCompletionContextType.MixinDirective;
                yield return SassCompletionContextType.VariableDefaultFlag;
                yield return SassCompletionContextType.WhileLoopDirective;
            }
        }

        public override IEnumerable<Completion> GetCompletions(SassCompletionContextType type, SassCompletionContext context)
        {
            return GetKeywords(type).Select(name => Keyword(name));
        }

        IEnumerable<string> GetKeywords(SassCompletionContextType type)
        {
            switch (type)
            {
                case SassCompletionContextType.AtDirectiveName:
                    yield return "@warn";
                    yield return "@debug";
                    break;
                case SassCompletionContextType.ConditionalDirective:
                    yield return "@if";
                    yield return "@else";
                    yield return "@else if";
                    break;
                case SassCompletionContextType.CssDirective:
                    yield return "@media";
                    yield return "@page";
                    break;
                case SassCompletionContextType.EachDirective:
                    yield return "@each";
                    break;
                case SassCompletionContextType.ExtendDirective:
                    yield return "@extend";
                    break;
                case SassCompletionContextType.ExtendDirectiveOptionalFlag:
                    yield return "!optional";
                    break;
                case SassCompletionContextType.ForLoopDirective:
                    yield return "@for";
                    break;
                case SassCompletionContextType.ForLoopFromKeyword:
                    yield return "from";
                    break;
                case SassCompletionContextType.ForLoopRangeKeyword:
                    yield return "to";
                    yield return "through";
                    break;
                case SassCompletionContextType.FunctionBody:
                    yield return "@return";
                    break;
                case SassCompletionContextType.FunctionDirective:
                    yield return "@function";
                    break;
                case SassCompletionContextType.ImportDirective:
                    yield return "@import";
                    break;
                case SassCompletionContextType.IncludeDirective:
                    yield return "@include";
                    break;
                case SassCompletionContextType.MixinDirective:
                    yield return "@mixin";
                    break;
                case SassCompletionContextType.VariableDefaultFlag:
                    yield return "!default";
                    break;
                case SassCompletionContextType.WhileLoopDirective:
                    yield return "@while";
                    break;
            }
        }
    }
}
