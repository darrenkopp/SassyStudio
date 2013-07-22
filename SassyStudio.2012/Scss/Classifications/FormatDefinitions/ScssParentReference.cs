using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Media;

namespace SassyStudio.Scss.Classifications
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.ParentReference)]
    [Name(ScssClassificationTypes.ParentReference)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssParentReference : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ImportingConstructor]
        public ScssParentReference(IEditorFormatMapService service)
            : base(service, "Operator")
        {
            DisplayName = "SCSS Parent Reference";
            ForegroundCustomizable = true;
        }
    }
}
