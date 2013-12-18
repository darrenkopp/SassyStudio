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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.FunctionReference)]
    [Name(ScssClassificationTypes.FunctionReference)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssFunctionReference : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScssFunctionReference()
            : base(foreground: true)
        {
            DisplayName = "SCSS Function Reference";
            ForegroundCustomizable = true;
        }

        protected override FormatColorStorage Light { get { return new FormatColorStorage { Foreground = Color.FromRgb(103, 103, 0) }; } }
        protected override FormatColorStorage Dark { get { return new FormatColorStorage { Foreground = Color.FromRgb(128, 128, 0) }; } }
    }
}
