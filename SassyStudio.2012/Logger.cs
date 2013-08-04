using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;

namespace SassyStudio
{
    public static class Logger
    {
        static readonly Lazy<IVsOutputWindowPane> _Pane = new Lazy<IVsOutputWindowPane>(() => SassyStudioPackage.Instance.GetOutputPane(new Guid("f1574ced-8144-4889-b7fd-485703faac9c"), "Sassy Studio"));
        private static IVsOutputWindowPane Pane { get { return _Pane.Value; } }


        public static void Log(string message, bool activatePane = false)
        {
            if (string.IsNullOrEmpty(message))
                return;

            try
            {
                var builder = new StringBuilder(message);
                builder.Insert(0, DateTime.Now.ToString("hh:mm:ss.fff tt': '"));
                builder.AppendLine();
                Pane.OutputString(builder.ToString());
                if (activatePane)
                    Pane.Activate();
            }
            catch
            {
                // woopsie
            }
        }

        public static void Log(Exception ex, string message = null, bool activatePane = true)
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(message))
                builder.AppendLine(message);

            if (ex != null)
            {
                builder
                    .Append("[").Append(ex.GetType().Name).Append("]")
                    .AppendLine(ex.Message);

                if (!string.IsNullOrEmpty(ex.StackTrace))
                    builder.AppendLine(ex.StackTrace);
            }

            if (builder.Length > 0)
                Log(builder.ToString(), activatePane);
        }
    }
}
