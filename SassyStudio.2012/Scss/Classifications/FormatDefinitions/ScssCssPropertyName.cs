using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
        public ScssCssPropertyName()
            : base(x => x.LineNumber, foreground: true)
        {
            DisplayName = "SCSS CSS Property Name";
            ForegroundCustomizable = true;
        }
    }
}
