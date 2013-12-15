using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

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
            : base("XML Doc Tag", "{75A05685-00A8-4DED-BAE5-E7A50BFA929A}", foreground: true)
        {
            DisplayName = "SCSS XML Doc Tag";
            ForegroundCustomizable = true;
        }
    }
}
