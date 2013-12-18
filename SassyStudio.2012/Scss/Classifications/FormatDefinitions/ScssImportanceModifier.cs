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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.ImportanceModifier)]
    [Name(ScssClassificationTypes.ImportanceModifier)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssImportanceModifier : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScssImportanceModifier()
            : base(foreground: true)
        {
            DisplayName = "SCSS Importance Modifier";
            ForegroundCustomizable = true;
            IsBold = true;
        }

        protected override FormatColorStorage Light { get { return new FormatColorStorage { Foreground = Color.FromRgb(81, 81, 163) }; } }
        protected override FormatColorStorage Dark { get { return new FormatColorStorage { Foreground = Color.FromRgb(128, 128, 192) }; } }
    }
}
