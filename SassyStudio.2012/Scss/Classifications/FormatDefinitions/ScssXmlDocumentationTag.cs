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
            : base(x => x.XmlDocTag, foreground: true)
        {
            DisplayName = "SCSS XML Doc Tag";
            ForegroundCustomizable = true;
        }
    }
}
