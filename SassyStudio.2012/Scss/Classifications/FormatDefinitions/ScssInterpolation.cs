using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Language.StandardClassification;
using System.Windows.Media;

namespace SassyStudio.Scss.Classifications
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.Interpolation)]
    [ClassificationType(ClassificationTypeNames = PredefinedClassificationTypeNames.Operator)]
    [Name(ScssClassificationTypes.Interpolation)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssInterpolation : ClassificationFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScssInterpolation()
        {
            DisplayName = "SCSS Interpolation";
            ForegroundCustomizable = true;
            BackgroundCustomizable = true;
            IsBold = true;
            ForegroundColor = Color.FromRgb(0x75, 0x75, 0x75);
        }
    }
}
