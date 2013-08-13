using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class FontFaceDirective : AtRuleDirective
    {
        protected override string RuleName { get { return "font-face"; } }
    }
}
