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
    sealed class ScssUserFunctionReference : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ImportingConstructor]
        public ScssUserFunctionReference(IEditorFormatMapService service)
            : base(service, "User Types(Delegates)", category: "{E0187991-B458-4F7E-8CA9-42C9A573B56C}", foreground: true)
        {
            DisplayName = "SCSS User Function Reference";
            ForegroundCustomizable = true;
        }
    }
}
