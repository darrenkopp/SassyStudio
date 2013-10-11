using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.StandardClassification;
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
        [ImportingConstructor]
        public ScssXmlDocumentationComment(IEditorFormatMapService service)
            : base(service, "XML Doc Comment", "{75A05685-00A8-4DED-BAE5-E7A50BFA929A}", foreground: true)
        {
            DisplayName = "SCSS Xml Doc Comment";
            ForegroundCustomizable = true;
        }
    }
}
