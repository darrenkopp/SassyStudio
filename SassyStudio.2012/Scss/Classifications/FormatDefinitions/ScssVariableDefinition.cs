using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SassyStudio.Scss.Classifications
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.VariableDefinition)]
    [ClassificationType(ClassificationTypeNames = PredefinedClassificationTypeNames.SymbolDefinition)]
    [Name(ScssClassificationTypes.VariableDefinition)]
    [Order(After = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssVariableDefinition : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScssVariableDefinition()
            : base(x => x.UserTypes, foreground: true)
        {
            DisplayName = "SCSS Variable Definition";
            ForegroundCustomizable = true;
        }
    }
}
