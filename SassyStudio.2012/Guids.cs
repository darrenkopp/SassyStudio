using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio
{
    static class Guids
    {
        public const string guidSassyStudioPkgString = "141b7a26-b79d-4ca0-9102-77e7cc7e0ec8";
        public const string guidSassyStudioCmdSetString = "9ba4ff19-dae9-47ba-af4a-2f13062e3990";

        public static readonly Guid guidSassyStudioCmdSet = new Guid(guidSassyStudioCmdSetString);
    }
}
