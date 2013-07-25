using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Language.StandardClassification;

namespace SassyStudio.Scss.Classifications
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.Interpolation)]
    [ClassificationType(ClassificationTypeNames = PredefinedClassificationTypeNames.Operator)]
    [Name(ScssClassificationTypes.Interpolation)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssInterpolation : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ImportingConstructor]
        public ScssInterpolation(IEditorFormatMapService service)
            : base(service, "Preprocessor Keyword", category: "{E0187991-B458-4F7E-8CA9-42C9A573B56C}", foreground: true, background: true)
        {
            DisplayName = "SCSS Interpolation";
            ForegroundCustomizable = true;
            BackgroundCustomizable = true;
            IsBold = true;
        }
    }
}
