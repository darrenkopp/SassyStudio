using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Language.StandardClassification;

namespace SassyStudio.Scss.Classifications
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.Comment)]
    [ClassificationType(ClassificationTypeNames = PredefinedClassificationTypeNames.Comment)]
    [Name(ScssClassificationTypes.Comment)]
    [Order(After = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssComment : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScssComment()
            : base("Comment", category: "{75A05685-00A8-4DED-BAE5-E7A50BFA929A}", foreground: true)
        {
            DisplayName = "SCSS Comment";
            ForegroundCustomizable = true;
        }
    }
}
