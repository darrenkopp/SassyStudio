using SassyStudio.Compiler.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class OptionalModifier : BangModifier
    {
        protected override string FlagName { get { return "optional"; } }
    }
}
