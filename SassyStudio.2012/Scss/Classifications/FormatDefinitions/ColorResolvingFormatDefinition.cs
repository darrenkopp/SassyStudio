using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell.Interop;

namespace SassyStudio.Scss.Classifications
{
    abstract class ColorResolvingFormatDefinition : ClassificationFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ColorResolvingFormatDefinition(string property, string category = null, bool foreground = false, bool background = false)
        {
            var storage = SassyStudioPackage.Instance.FontsAndColorsStorage;
            var categoryId = Guid.Empty;
            if (!string.IsNullOrEmpty(category))
                categoryId = Guid.Parse(category);

            var cr = storage.OpenCategory(ref categoryId, (uint)(__FCSTORAGEFLAGS.FCSF_READONLY | __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS));
            try
            {
                if (cr != 0) return;
                ColorableItemInfo[] colors = new ColorableItemInfo[1];
                var result = storage.GetItem(property, colors);
                if (result == 0)
                {
                    var color = colors[0];
                    if (foreground) ForegroundColor = ParseColor(color.crForeground);
                    if (background) BackgroundColor = ParseColor(color.crBackground);
                }
            }
            finally
            {
                storage.CloseCategory();
            }
        }

        private Color ParseColor(uint color)
        {
            var dcolor = System.Drawing.ColorTranslator.FromOle(Convert.ToInt32(color));
            return Color.FromArgb(dcolor.A, dcolor.R, dcolor.G, dcolor.B);
        }
    }
}
