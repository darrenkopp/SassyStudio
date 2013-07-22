using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SassyStudio.Scss.Classifications
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.VariableDefinition)]
    [Name(ScssClassificationTypes.VariableDefinition)]
    [Order(After = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssVariableDefinition : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ImportingConstructor]
        public ScssVariableDefinition(IEditorFormatMapService service)
            : base(service, "CSS Property Name")
        {
            DisplayName = "SCSS Variable Definition";
            ForegroundCustomizable = true;
        }
    }
}
