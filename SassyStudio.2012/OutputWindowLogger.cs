using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio
{
    [Export(typeof(IOutputWindowLogger))]
    public class OutputWindowLogger : IOutputWindowLogger
    {
        public void Log(string message)
        {
            Logger.Log(message);
        }
    }
}
