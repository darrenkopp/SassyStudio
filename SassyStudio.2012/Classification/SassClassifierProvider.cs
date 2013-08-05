using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using SassyStudio.Compiler.Parsing;
using SassyStudio.Editor;

namespace SassyStudio.Classification
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("SCSS")]
    class SassClassifierProvider : IClassifierProvider
    {
        [Import, System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal IClassificationTypeRegistryService Registry { get; set; }

        [Import]
        internal IParserFactory ParserFactory { get; set; }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new SassClassifier(buffer, Registry, ParserFactory));
        }
    }
}
