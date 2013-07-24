using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell.Interop;

namespace SassyStudio.Scss.Classifications
{
    abstract class ColorResolvingFormatDefinition : ClassificationFormatDefinition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ColorResolvingFormatDefinition(IEditorFormatMapService service, string property, string category = null, bool foreground = false, bool background = false)
        {
            if (foreground)
            {
                ForegroundBrush = GetBrush(service, category, property, ForegroundBrushId);

                if (ForegroundBrush == null)
                    ForegroundColor = GetColor(service, category, property, ForegroundColorId);

                if (ForegroundBrush == null && ForegroundColor.HasValue)
                    ForegroundBrush = new SolidColorBrush(ForegroundColor.Value);
            }

            if (background)
            {
                BackgroundBrush = GetBrush(service, category, property, BackgroundBrushId);

                if (BackgroundBrush == null)
                    BackgroundColor = GetColor(service, category, property, BackgroundColorId);

                if (BackgroundBrush == null && BackgroundColor.HasValue)
                    BackgroundBrush = new SolidColorBrush(BackgroundColor.Value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static Color? GetColor(IEditorFormatMapService service, string category, string property, string resourceName)
        {
            var mapping = GetMapping(service, category, property, resourceName);

            if (mapping != null)
            {
                var resources = mapping.GetProperties(property);

                if (resources.Contains(resourceName))
                    return (Color)resources[resourceName];
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static Brush GetBrush(IEditorFormatMapService service, string category, string property, string resourceName)
        {
            var mapping = GetMapping(service, category, property, resourceName);

            if (mapping != null)
            {
                var resources = mapping.GetProperties(property);

                if (resources.Contains(resourceName))
                    return resources[resourceName] as Brush;
            }

            return null;
        }

        private static IEditorFormatMap GetMapping(IEditorFormatMapService service, string category, string property, string resourceName)
        {
            var mapping = service.GetEditorFormatMap(category);
            var resources = mapping.GetProperties(property);
            if (resources.Contains(resourceName))
                return mapping;

            return null;
        }
    }
}
