using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio
{
    public interface ISassStylesheet
    {
        ParseItemList Children { get; }
        ISassDocument Owner { get; set; }
    }
}
