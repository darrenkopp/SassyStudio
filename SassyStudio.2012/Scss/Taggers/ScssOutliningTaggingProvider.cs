using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace SassyStudio.Scss.Taggers
{
    //[Export(typeof(ITaggerProvider))]
    //[TagType(typeof(IOutliningRegionTag))]
    //[ContentType(ScssContentTypeDefinition.ScssContentType)]
    class ScssOutliningTaggingProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer != null)
                return buffer.Properties.GetOrCreateSingletonProperty<ScssOutliningTagger>(() => new ScssOutliningTagger(buffer)) as ITagger<T>;

            return null;
        }
    }
}
