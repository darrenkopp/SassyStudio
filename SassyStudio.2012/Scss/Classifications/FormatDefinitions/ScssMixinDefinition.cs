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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.MixinDefinition)]
    [Name(ScssClassificationTypes.MixinDefinition)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssMixinDefinition : ClassificationFormatDefinition
    {
        public ScssMixinDefinition()
        {
            DisplayName = "SCSS Mixin Definition";
            ForegroundCustomizable = true;
        }
    }
}
