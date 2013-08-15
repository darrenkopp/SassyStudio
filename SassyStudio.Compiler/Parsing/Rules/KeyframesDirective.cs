using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing.Rules
{
    public class KeyframesDirective : AtRuleDirective
    {
        protected override string RuleName { get { return "keyframes"; } }
    }
}
