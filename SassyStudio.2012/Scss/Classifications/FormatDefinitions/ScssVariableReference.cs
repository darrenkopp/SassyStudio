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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.VariableReference)]
    [Name(ScssClassificationTypes.VariableReference)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssVariableReference : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScssVariableReference()
            : base(foreground: true)
        {
            DisplayName = "SCSS Variable Reference";
            ForegroundCustomizable = true;
        }

        protected override FormatColorStorage Light { get { return new FormatColorStorage { Foreground = Color.FromRgb(170, 78, 228) }; } }
        protected override FormatColorStorage Dark { get { return new FormatColorStorage { Foreground = Color.FromRgb(238, 225, 144) }; } }
    }
}
