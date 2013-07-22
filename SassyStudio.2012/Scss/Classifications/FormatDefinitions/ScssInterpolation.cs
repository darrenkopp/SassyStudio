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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.Interpolation)]
    [Name(ScssClassificationTypes.Interpolation)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssInterpolation : ClassificationFormatDefinition
    {
        public ScssInterpolation()
        {
            DisplayName = "SCSS Interpolation";
            ForegroundCustomizable = true;
            IsBold = true;
        }
    }
}
