using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace SassyStudio.Scss.Classifications
{
    abstract class ColorResolvingFormatDefinition : ClassificationFormatDefinition
    {
        readonly Func<DefaultColorsStorage, Tuple<Color?, Color?>> Accessor;
        protected ColorResolvingFormatDefinition(Func<DefaultColorsStorage,Tuple<Color?,Color?>> accessor, bool foreground = false, bool background = false)
        {
            Accessor = accessor;
            StyleForeground = foreground;
            StyleBackground = background;

            SassyStudioPackage.Instance.FontsAndColorsStorage.ColorsChanged += OnColorsChanged;

            Apply();
        }
        
        bool StyleForeground { get; set; }
        bool StyleBackground { get; set; }

        private void Apply()
        {
            var value = Accessor(SassyStudioPackage.Instance.FontsAndColorsStorage);
            if (value == null)
                return;

            if (StyleForeground && ForegroundColor == null)
                ForegroundColor = value.Item1;

            if (StyleBackground && BackgroundColor == null)
                BackgroundColor = value.Item2;
        }

        private void OnColorsChanged(object sender, EventArgs e)
        {
            Apply();
        }
    }
}
