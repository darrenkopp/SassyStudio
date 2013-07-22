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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.UserFunctionReference)]
    [Name(ScssClassificationTypes.UserFunctionReference)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssUserFunctionReference : ClassificationFormatDefinition
    {
        public ScssUserFunctionReference()
        {
            DisplayName = "SCSS User Function Reference";
            ForegroundCustomizable = true;
        }
    }
}
