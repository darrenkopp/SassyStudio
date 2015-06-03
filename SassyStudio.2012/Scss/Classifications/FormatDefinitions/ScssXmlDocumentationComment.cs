using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Media;

namespace SassyStudio.Scss.Classifications
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.XmlDocumentationComment)]
    [Name(ScssClassificationTypes.XmlDocumentationComment)]
    [Order(After = Priority.Default)]
    [UserVisible(true)]
    class ScssXmlDocumentationComment : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScssXmlDocumentationComment()
            : base(foreground: true)
        {
            DisplayName = "SCSS XML Doc Comment";
            ForegroundCustomizable = true;
        }

        protected override FormatColorStorage Light { get { return new FormatColorStorage { Foreground = Color.FromRgb(59, 139, 59) }; } }
        protected override FormatColorStorage Dark { get { return new FormatColorStorage { Foreground = Color.FromRgb(103, 197, 103) }; } }
    }
}
