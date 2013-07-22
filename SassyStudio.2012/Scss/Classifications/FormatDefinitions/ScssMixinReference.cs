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
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.MixinReference)]
    [Name(ScssClassificationTypes.MixinReference)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssMixinReference : ClassificationFormatDefinition
    {
        public ScssMixinReference()
        {
            DisplayName = "SCSS Mixin Reference";
            ForegroundCustomizable = true;
        }
    }
}
