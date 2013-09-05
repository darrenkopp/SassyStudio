using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionValueProvider))]
    class MixinsProvider : ICompletionValueProvider
    {
        public IEnumerable<SassCompletionContextType> SupportedContexts
        {
            get
            {
                yield return SassCompletionContextType.IncludeDirective;
                yield return SassCompletionContextType.IncludeDirectiveMixinName;
            }
        }

        public IEnumerable<ICompletionValue> GetCompletions(SassCompletionContextType type, ICompletionContext context)
        {
            switch (type)
            {
                case SassCompletionContextType.IncludeDirective:
                case SassCompletionContextType.IncludeDirectiveMixinName:
                    return context.Cache.GetMixins(context.Position);
            }

            return Enumerable.Empty<ICompletionValue>();
        }
    }
}
