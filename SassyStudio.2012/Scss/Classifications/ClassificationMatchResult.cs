using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace SassyStudio.Scss.Classifications
{
    class ClassificationMatchResult
    {
        private ClassificationMatchResult()
        {
        }

        public IClassificationType Type { get; private set; }
        public int Index { get; private set; }
        public int Length { get; private set; }

        public static ClassificationMatchResult Create(IClassificationType type, int index, int length)
        {
            return new ClassificationMatchResult
            {
                Type = type,
                Index = index,
                Length = length
            };
        }
    }
}
