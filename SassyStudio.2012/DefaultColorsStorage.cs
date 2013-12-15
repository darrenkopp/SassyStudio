using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace SassyStudio
{
    class DefaultColorsStorage
    {
        public DefaultColorsStorage(IVsFontAndColorStorage storage)
        {
            // text editor stuff
            InCategory(storage, "{A27B4E24-A735-4D1D-B8E7-9716E1E3D8E0}", () =>
            {
                Comment = TryGetItem(storage, "CSS Comment");
                LineNumber = TryGetItem(storage, "Line Number");
                Keyword = TryGetItem(storage, "CSS Keyword");
                StringColors = TryGetItem(storage, "String");
                StringCSharpVerbatim = TryGetItem(storage, "String(C# @ Verbatim)");
                SymbolDefinition = TryGetItem(storage, "Symbol Definition");
                SymbolReference = TryGetItem(storage, "Symbol Reference");
                UserTypes = TryGetItem(storage, "User Types");
                UserTypesDelegate = TryGetItem(storage, "User Types(Delegates)");
                UserTypesInterfaces = TryGetItem(storage, "User Types(Interfaces)");
                UserTypesTypeParameters = TryGetItem(storage, "User Types(Type parameters)");
                UserTypesValueTypes = TryGetItem(storage, "User Types(Value types)");
                XmlDocComment = TryGetItem(storage, "XML Doc Comment");
                XmlDocTag = TryGetItem(storage, "XML Doc Tag");
            });

            OnColorsChanged();
        }

        public event EventHandler ColorsChanged;

        public Tuple<Color?, Color?> Comment { get; private set; }
        public Tuple<Color?, Color?> LineNumber { get; private set; }
        public Tuple<Color?, Color?> Keyword { get; private set; }
        public Tuple<Color?, Color?> StringCSharpVerbatim { get; private set; }
        public Tuple<Color?, Color?> StringColors { get; private set; }
        public Tuple<Color?, Color?> SymbolDefinition { get; private set; }
        public Tuple<Color?, Color?> SymbolReference { get; private set; }
        public Tuple<Color?, Color?> UserTypes { get; private set; }
        public Tuple<Color?, Color?> UserTypesDelegate { get; private set; }
        public Tuple<Color?, Color?> UserTypesInterfaces { get; private set; }
        public Tuple<Color?, Color?> UserTypesTypeParameters { get; private set; }
        public Tuple<Color?, Color?> UserTypesValueTypes { get; private set; }
        public Tuple<Color?, Color?> XmlDocComment { get; private set; }
        public Tuple<Color?, Color?> XmlDocTag { get; private set; }

        private void OnColorsChanged()
        {
            var handler = ColorsChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        static void InCategory(IVsFontAndColorStorage storage, string category, Action callback)
        {
            if (string.IsNullOrEmpty(category)) return;

            var categoryId = Guid.Parse(category);
            var hresult = storage.OpenCategory(ref categoryId, (uint)(__FCSTORAGEFLAGS.FCSF_READONLY | __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS));

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
            ColorableItemInfo[] colors = new ColorableItemInfo[1];
            var hresult = storage.GetItem(item, colors);
            if (hresult == 0)
                return Tuple.Create<Color?,Color?>(ParseColor(colors[0].crForeground), ParseColor(colors[0].crBackground));

            return Tuple.Create<Color?,Color?>(null,null);
        }

        static Color ParseColor(uint color)
        {
            var dcolor = System.Drawing.ColorTranslator.FromOle(Convert.ToInt32(color));
            return Color.FromArgb(dcolor.A, dcolor.R, dcolor.G, dcolor.B);
        }
    }
}
