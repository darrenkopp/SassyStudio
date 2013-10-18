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
        public ImportContainer(ParseItem source, IIntellisenseCache cache)
        {
            Cache = cache;
            Start = source.Start;
            End = source.End;
        }

        public int Start { get; private set; }
        public int End { get; private set; }

        public void Add(ParseItem item, ITextProvider text)
        {
            throw new NotSupportedException("you can't add items to this type of container.");
        }

        public bool IsApplicableTo(int position)
        {
            return position > End;
        }

        public IEnumerable<ICompletionValue> GetVariables(int position)
        {
            return SafeCache(c => c.GetVariables());
        }

        public IEnumerable<ICompletionValue> GetFunctions(int position)
        {
            return SafeCache(c => c.GetFunctions());
        }

        public IEnumerable<ICompletionValue> GetMixins(int position)
        {
            return SafeCache(c => c.GetMixins());
        }

        private IEnumerable<ICompletionValue> SafeCache(Func<IIntellisenseCache, IEnumerable<ICompletionValue>> callback)
        {
            if (Cache == null)
                return Enumerable.Empty<ICompletionValue>();

            return callback(Cache);
        }
    }
}
