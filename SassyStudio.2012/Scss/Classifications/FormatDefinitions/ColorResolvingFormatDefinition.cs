using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace SassyStudio.Scss.Classifications
{
    abstract class ColorResolvingFormatDefinition : ClassificationFormatDefinition
    {
        static readonly Lazy<IVsFontAndColorStorage> _Storage = new Lazy<IVsFontAndColorStorage>(() => SassyStudioPackage.GetGlobalService(typeof(SVsFontAndColorStorage)) as IVsFontAndColorStorage, true);
        static readonly Lazy<Tuple<Color?, Color?>> _PlainText = new Lazy<Tuple<Color?, Color?>>(() => TryGetItem(_Storage.Value, "Plain Text"), true);
        
        protected ColorResolvingFormatDefinition(bool foreground = false, bool background = false)
        {
            StyleForeground = foreground;
            StyleBackground = background;

            Apply();
        }

        bool StyleForeground { get; set; }
        bool StyleBackground { get; set; }

        protected abstract FormatColorStorage Light { get; }
        protected abstract FormatColorStorage Dark { get; }

        private void Apply()
        {
            var value = _PlainText.Value;
            if (value == null)
                return;

            if (StyleForeground && ForegroundColor == null)
                ForegroundColor = Pick(value.Item2, Light.Foreground, Dark.Foreground);

            if (StyleBackground && BackgroundColor == null)
                BackgroundColor = Pick(value.Item2, Light.Background, Dark.Background);
        }

        static Color? Pick(Color? background, Color? light, Color? dark)
        {
            if (background == null) return null;

            byte r = background.Value.R, g = background.Value.G, b = background.Value.B;
            // HSP equation from http://alienryderflex.com/hsp.html
            var hsp = Math.Sqrt(
                  0.299 * (r * r) +
                  0.587 * (g * g) +
                  0.114 * (b * b)
            );

            return hsp > 127.5 ? light : dark;
        }

        static void InCategory(IVsFontAndColorStorage storage, Guid category, Action callback)
        {
            var hresult = storage.OpenCategory(ref category, (uint)(__FCSTORAGEFLAGS.FCSF_READONLY | __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS));

            try
            {
                if (hresult == 0)
                    callback();
            }
            finally
            {
                storage.CloseCategory();
            }
        }

        static Tuple<Color?, Color?> TryGetItem(IVsFontAndColorStorage storage, string item)
        {
            Tuple<Color?, Color?> result = null;
            // load specific category to prevent our own format classifications being loaded
            InCategory(storage, Microsoft.VisualStudio.Editor.DefGuidList.guidTextEditorFontCategory, () =>
            {
                ColorableItemInfo[] colors = new ColorableItemInfo[1];
                var hresult = storage.GetItem(item, colors);
                if (hresult == 0)
                {
                    result = Tuple.Create<Color?, Color?>(ParseColor(colors[0].crForeground), ParseColor(colors[0].crBackground));
                }
                else
                {
                    result = Tuple.Create<Color?, Color?>(null, null);
                }
            });

            return result;
        }

        static Color ParseColor(uint color)
        {
            var dcolor = System.Drawing.ColorTranslator.FromOle(Convert.ToInt32(color));
            return Color.FromArgb(dcolor.A, dcolor.R, dcolor.G, dcolor.B);
        }
    }
}
