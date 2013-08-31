using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    class ImportContainer : IIntellisenseContainer
    {
        readonly IIntellisenseCache Cache;
        public ImportContainer(IIntellisenseCache cache)
        {
            Cache = cache;
        }

        public void Add(ParseItem item, ITextProvider text)
        {
            throw new NotSupportedException("you can't add items to this type of container.");
        }

        public IEnumerable<ICompletionValue> GetVariables(int position)
        {
            return Cache.GetVariables();
        }

        public IEnumerable<ICompletionValue> GetFunctions(int position)
        {
            return Cache.GetFunctions();
        }

        public IEnumerable<ICompletionValue> GetMixins(int position)
        {
            return Cache.GetMixins();
        }
    }
}
