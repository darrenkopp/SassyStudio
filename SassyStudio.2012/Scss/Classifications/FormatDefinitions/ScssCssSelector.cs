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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.CssSelector)]
    [Name(ScssClassificationTypes.CssSelector)]
    [Order(After = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssCssSelector : ClassificationFormatDefinition
    {
        public ScssCssSelector()
        {
            DisplayName = "SCSS CSS Selector";
        }
    }
}
