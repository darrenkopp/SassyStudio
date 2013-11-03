using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace SassyStudio.Editor.Intellisense
{
    interface ICssSchemaLoader
    {
        ICssSchema Load();
    }

    [Export(typeof(ICssSchemaLoader))]
    class DefaultCssSchemaLoader : ICssSchemaLoader
    {
        public ICssSchema Load()
        {
            try
            {
                var path = Path.Combine(SchemaDirectory, "css-main.xml");
                var file = new FileInfo(path);
                if (!file.Exists)
                    return null;

                var document = XDocument.Load(path);

                return CssSchema.Parse(document, file.Directory);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Failed to load schema.");
                return null;
            }
        }

        string VisualStudioInstallPath
        {
            get
            {
                return new ShellSettingsManager(SassyStudioPackage.Instance)
                    .GetReadOnlySettingsStore(SettingsScope.Configuration)
                    .GetString(string.Empty, "ShellFolder");
            }
        }

        string SchemaDirectory
        {
            get
            {
                var localeId = SassyStudioPackage.Instance.LocaleId;
                if (localeId != 0)
                {
                    // we'll assume this is the path moving forward
                    var vs2013 = Path.Combine(VisualStudioInstallPath, "Common7", "IDE", "CommonExtensions", "Microsoft", "Web", "Schemas", localeId.ToString(), "css");
                    if (Directory.Exists(vs2013))
                        return vs2013;

                    var vs2012 = Path.Combine(VisualStudioInstallPath, "Common7", "Packages", localeId.ToString(), "schemas", "css");
                    if (Directory.Exists(vs2012))
                        return vs2012;
                }

                return string.Empty;
            }
        }
    }
}
