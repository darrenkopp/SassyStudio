using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Media;

namespace SassyStudio.Scss.Classifications
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.VariableDefinition)]
    [ClassificationType(ClassificationTypeNames = PredefinedClassificationTypeNames.SymbolDefinition)]
    [Name(ScssClassificationTypes.VariableDefinition)]
    [Order(After = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssVariableDefinition : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScssVariableDefinition()
            : base(foreground: true)
        {
            DisplayName = "SCSS Variable Definition";
            ForegroundCustomizable = true;
        }

        protected override FormatColorStorage Light { get { return new FormatColorStorage { Foreground = Color.FromRgb(69, 143, 166) }; } }
        protected override FormatColorStorage Dark { get { return new FormatColorStorage { Foreground = Color.FromRgb(133, 187, 204) }; } }
    }
}
