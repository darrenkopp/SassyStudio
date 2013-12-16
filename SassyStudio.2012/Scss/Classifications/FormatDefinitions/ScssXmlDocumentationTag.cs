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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.XmlDocumentationTag)]
    [Name(ScssClassificationTypes.XmlDocumentationTag)]
    [Order(After = Priority.Default)]
    [UserVisible(true)]
    class ScssXmlDocumentationTag : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScssXmlDocumentationTag()
            : base(foreground: true)
        {
            DisplayName = "SCSS XML Doc Tag";
            ForegroundCustomizable = true;
        }

        protected override FormatColorStorage Light { get { return new FormatColorStorage { Foreground = Color.FromRgb(128, 128, 128) }; } }
        protected override FormatColorStorage Dark { get { return new FormatColorStorage { Foreground = Color.FromRgb(128, 128, 128) }; } }
    }
}
