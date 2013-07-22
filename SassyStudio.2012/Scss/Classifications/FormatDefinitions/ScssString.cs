using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SassyStudio.Scss.Classifications
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.StringValue)]
    [Name(ScssClassificationTypes.StringValue)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssString : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ImportingConstructor]
        public ScssString(IEditorFormatMapService service)
            : base(service, "CSS String Value")
        {
            DisplayName = "SCSS String";
            ForegroundCustomizable = true;
        }
    }
}
