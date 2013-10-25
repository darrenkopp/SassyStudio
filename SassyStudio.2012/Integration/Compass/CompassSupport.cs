using System.IO;
using System.Linq;

namespace SassyStudio.Integration.Compass
{
    class CompassSupport
    {
        public static string CompassBatchFile { get { return Path.Combine(SassyStudioPackage.Instance.Options.Scss.RubyInstallPath, "bin", "compass.bat"); } }

        public static bool IsCompassInstalled
        {
            get
            {
                var ruby = SassyStudioPackage.Instance.Options.Scss.RubyInstallPath;
                if (string.IsNullOrEmpty(ruby) || !Directory.Exists(ruby))
                    return false;

                return File.Exists(Path.Combine(ruby, "bin", "compass.bat"));
            }
        }

        public static bool IsInCompassProject(DirectoryInfo current)
        {
            if (current.Parent == null || current.Root.Equals(current))
                return false;

            if (current.EnumerateFiles("config.rb").Any())
                return true;

            return IsInCompassProject(current.Parent);
        }
    }
}
