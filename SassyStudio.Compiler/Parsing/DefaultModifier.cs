using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class DefaultModifier : BangModifier
    {
        protected override string FlagName { get { return "default"; } }
    }
}
