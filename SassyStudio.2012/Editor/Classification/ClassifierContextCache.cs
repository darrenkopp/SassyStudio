using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Classification
{
    static class ClassifierContextCache
    {
        static readonly ConcurrentDictionary<SassClassifierType, IClassifierContext> Cache = new ConcurrentDictionary<SassClassifierType, IClassifierContext>();

        public static IClassifierContext Get(SassClassifierType type)
        {
            return Cache.GetOrAdd(type, t => new ClassifierContext(type));
        }
    }
}
