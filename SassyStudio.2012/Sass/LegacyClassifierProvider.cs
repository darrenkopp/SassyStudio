using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SassyStudio.Sass.Classifications
{
    [Export(typeof(IClassifierProvider))]
    [ContentType(SassContentTypeDefinition.SassContentType)]
    class LegacyClassifierProvider : IClassifierProvider
    {
        [Import, System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal IClassificationTypeRegistryService Registry { get; set; }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            if (buffer != null)
                return buffer.Properties.GetOrCreateSingletonProperty(() => new LegacyClassifier(Registry));

            return null;
        }
    }
}
