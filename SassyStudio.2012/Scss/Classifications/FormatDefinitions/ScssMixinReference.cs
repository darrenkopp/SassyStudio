using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Language.StandardClassification;

namespace SassyStudio.Scss.Classifications
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.MixinReference)]
    [ClassificationType(ClassificationTypeNames = PredefinedClassificationTypeNames.SymbolReference)]
    [Name(ScssClassificationTypes.MixinReference)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssMixinReference : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ImportingConstructor]
        public ScssMixinReference(IEditorFormatMapService service)
            : base(service, "User Types(Interfaces)", category: "{E0187991-B458-4F7E-8CA9-42C9A573B56C}", foreground: true)
        {
            DisplayName = "SCSS Mixin Reference";
            ForegroundCustomizable = true;
        }
    }
}
