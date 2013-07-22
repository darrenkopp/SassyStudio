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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.ImportanceModifier)]
    [Name(ScssClassificationTypes.ImportanceModifier)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssImportanceModifier : ClassificationFormatDefinition
    {
        public ScssImportanceModifier()
        {
            DisplayName = "SCSS Importance Modifier";
            ForegroundCustomizable = true;
            IsBold = true;
        }
    }
}
