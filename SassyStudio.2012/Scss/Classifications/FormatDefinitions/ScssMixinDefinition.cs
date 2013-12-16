﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Language.StandardClassification;
using System.Windows.Media;

namespace SassyStudio.Scss.Classifications
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ScssClassificationTypes.MixinDefinition)]
    [ClassificationType(ClassificationTypeNames = PredefinedClassificationTypeNames.SymbolDefinition)]
    [Name(ScssClassificationTypes.MixinDefinition)]
    [Order(Before = Priority.Default)]
    [UserVisible(true)]
    sealed class ScssMixinDefinition : ColorResolvingFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ScssMixinDefinition()
            : base(foreground: true)
        {
            DisplayName = "SCSS Mixin Definition";
            ForegroundCustomizable = true;
        }

        protected override FormatColorStorage Light { get { return new FormatColorStorage { Foreground = Color.FromRgb(143, 192, 110) }; } }
        protected override FormatColorStorage Dark { get { return new FormatColorStorage { Foreground = Color.FromRgb(184, 215, 163) }; } }
    }
}
