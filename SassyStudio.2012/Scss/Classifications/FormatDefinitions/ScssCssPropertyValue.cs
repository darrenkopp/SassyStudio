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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.CssPropertyValue)]
    [Name(ScssClassificationTypes.CssPropertyValue)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssCssPropertyValue : ClassificationFormatDefinition
    {
        public ScssCssPropertyValue()
        {
            DisplayName = "SCSS CSS Property Value";
        }
    }
}
