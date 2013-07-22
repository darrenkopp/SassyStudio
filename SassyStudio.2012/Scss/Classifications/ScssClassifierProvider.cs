using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SassyStudio.Scss.Classifications
{
    [Export(typeof(IClassifierProvider))]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    class ScssClassifierProvider : IClassifierProvider
    {
        [Import, System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal IClassificationTypeRegistryService Registry { get; set; }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            if (buffer != null)
                return buffer.Properties.GetOrCreateSingletonProperty<ScssClassifier>(() => new ScssClassifier(Registry));

            return null;
        }
    }
}
