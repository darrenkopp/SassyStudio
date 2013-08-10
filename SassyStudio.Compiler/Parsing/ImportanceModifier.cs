using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SassyStudio.Compiler.Lexing;

namespace SassyStudio.Compiler.Parsing
{
    public class ImportanceModifier : BangModifier
    {
        protected override string FlagName { get { return "important"; } }
    }
}
