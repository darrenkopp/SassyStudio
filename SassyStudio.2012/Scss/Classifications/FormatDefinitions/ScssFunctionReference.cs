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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.FunctionReference)]
    [Name(ScssClassificationTypes.FunctionReference)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssFunctionReference : ClassificationFormatDefinition
    {
        public ScssFunctionReference()
        {
            DisplayName = "SCSS Function Reference";
            ForegroundCustomizable = true;
        }
    }
}
