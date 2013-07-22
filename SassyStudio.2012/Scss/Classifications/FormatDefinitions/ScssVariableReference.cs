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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.VariableReference)]
    [Name(ScssClassificationTypes.VariableReference)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssVariableReference : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ImportingConstructor]
        public ScssVariableReference(IEditorFormatMapService service)
            : base(service, "CSS Property Value")
        {
            DisplayName = "SCSS Variable Reference";
            ForegroundCustomizable = true;
        }
    }
}
