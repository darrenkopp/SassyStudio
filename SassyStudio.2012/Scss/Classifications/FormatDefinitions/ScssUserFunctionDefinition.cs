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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.UserFunctionDefinition)]
    [Name(ScssClassificationTypes.UserFunctionDefinition)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssUserFunctionDefinition : ClassificationFormatDefinition
    {
        public ScssUserFunctionDefinition()
        {
            DisplayName = "SCSS User Function Definition";
            ForegroundCustomizable = true;
        }
    }
}
