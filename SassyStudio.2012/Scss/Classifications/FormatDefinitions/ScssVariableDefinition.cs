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
    [ClassificationType(ClassificationTypeNames = PredefinedClassificationTypeNames.SymbolDefinition)]
    [Name(ScssClassificationTypes.VariableDefinition)]
    [Order(After = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssVariableDefinition : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ImportingConstructor]
        public ScssVariableDefinition(IEditorFormatMapService service)
            : base(service, "User Types", category: "{E0187991-B458-4F7E-8CA9-42C9A573B56C}", foreground: true)
        {
            DisplayName = "SCSS Variable Definition";
            ForegroundCustomizable = true;
        }
    }
}
