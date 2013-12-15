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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.Keyword)]
    [ClassificationType(ClassificationTypeNames = PredefinedClassificationTypeNames.Keyword)]
    [Name(ScssClassificationTypes.Keyword)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssKeyword : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScssKeyword()
            : base(x => x.UserTypesTypeParameters, foreground: true)
        {
            DisplayName = "SCSS Keyword";
            ForegroundCustomizable = true;
        }
    }
}
