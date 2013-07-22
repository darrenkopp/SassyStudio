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
    sealed class ScssVariableReference : ClassificationFormatDefinition
    {
        public ScssVariableReference()
        {
            DisplayName = "SCSS Variable Reference";
            ForegroundCustomizable = true;
        }
    }
}
