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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.Comment)]
    [Name(ScssClassificationTypes.Comment)]
    [Order(After = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssComment : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScssComment()
            : base(foreground: true)
        {
            DisplayName = "SCSS Comment";
            ForegroundCustomizable = true;
        }

        protected override FormatColorStorage Light { get { return new FormatColorStorage { Foreground = Color.FromRgb(74, 175, 74) }; } }
        protected override FormatColorStorage Dark { get {  return new FormatColorStorage { Foreground = Color.FromRgb(107, 193, 107) }; } }
    }
}
