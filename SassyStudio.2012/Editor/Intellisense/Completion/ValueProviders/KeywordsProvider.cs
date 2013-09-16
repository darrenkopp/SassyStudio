using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionValueProvider))]
    class KeywordsProvider : ICompletionValueProvider
    {
        readonly ICssSchemaManager SchemaManager;

        [ImportingConstructor]
        public KeywordsProvider(ICssSchemaManager schemaManager)
        {
            SchemaManager = schemaManager;
        }

        public IEnumerable<SassCompletionContextType> SupportedContexts
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
                yield return SassCompletionContextType.KeyframesDirective;
                yield return SassCompletionContextType.KeyframesNamedRange;
                yield return SassCompletionContextType.MixinDirective;
                yield return SassCompletionContextType.VariableDefaultFlag;
                yield return SassCompletionContextType.WhileLoopDirective;
            }
        }

        public IEnumerable<ICompletionValue> GetCompletions(SassCompletionContextType type, ICompletionContext context)
        {
            return GetKeywords(type).Select(name => new KeywordCompletionValue(name));
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
                    foreach (var cssDirective in GetCssDirectives())
                        yield return cssDirective;
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
                case SassCompletionContextType.KeyframesDirective:
                    yield return "@keyframes";
                    break;
                case SassCompletionContextType.KeyframesNamedRange:
                    yield return "from";
                    yield return "to";
                    break;
                case SassCompletionContextType.MixinDirective:
                    yield return "@mixin";
                    break;
                case SassCompletionContextType.MixinBody:
                    yield return "@content";
                    break;
                case SassCompletionContextType.VariableDefaultFlag:
                    yield return "!default";
                    break;
                case SassCompletionContextType.WhileLoopDirective:
                    yield return "@while";
                    break;
            }
        }

        public IEnumerable<string> GetCssDirectives()
        {
            var schema = SchemaManager.CurrentSchema;
            if (schema == null) 
                return Enumerable.Empty<string>();

            return schema.GetDirectives().Select(x => "@" + x.Name);
        }
    }
}
