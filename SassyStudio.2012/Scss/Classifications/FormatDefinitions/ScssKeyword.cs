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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.Keyword)]
    [ClassificationType(ClassificationTypeNames = PredefinedClassificationTypeNames.Keyword)]
    [Name(ScssClassificationTypes.Keyword)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssKeyword : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScssKeyword()
            : base(foreground: true)
        {
            DisplayName = "SCSS Keyword";
            ForegroundCustomizable = true;
        }

        protected override FormatColorStorage Light { get { return new FormatColorStorage { Foreground = Color.FromRgb(0, 102, 204) }; } }
        protected override FormatColorStorage Dark { get { return new FormatColorStorage { Foreground = Color.FromRgb(86, 156, 214) }; } }
    }
}
