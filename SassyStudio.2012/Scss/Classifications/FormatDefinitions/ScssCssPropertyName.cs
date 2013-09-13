using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SassyStudio.Scss.Classifications
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.CssPropertyName)]
    [Name(ScssClassificationTypes.CssPropertyName)]
    [Order(After = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssCssPropertyName : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ImportingConstructor]
        public ScssCssPropertyName(IEditorFormatMapService service, IClassificationFormatMapService c)
            : base(service, "HTML Element Name", category: "{75A05685-00A8-4DED-BAE5-E7A50BFA929A}", foreground: true)
        {
            DisplayName = "SCSS CSS Property Name";
            ForegroundCustomizable = true;
        }
    }
}
