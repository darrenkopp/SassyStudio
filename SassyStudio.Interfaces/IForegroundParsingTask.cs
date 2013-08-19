using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler;

namespace SassyStudio
{
    public interface IForegroundParsingTask
    {
        ISassStylesheet Parse(IParsingRequest request);
    }
}
