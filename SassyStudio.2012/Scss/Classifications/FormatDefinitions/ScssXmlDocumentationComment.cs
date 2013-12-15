using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

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
            : base(x => x.XmlDocComment, foreground: true)
        {
            DisplayName = "SCSS Xml Doc Comment";
            ForegroundCustomizable = true;
        }
    }
}
