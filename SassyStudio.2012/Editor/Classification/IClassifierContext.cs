using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace SassyStudio.Editor.Classification
{
    interface IClassifierContext
    {
        IClassificationType GetClassification(IClassificationTypeRegistryService registry);
    }
}
