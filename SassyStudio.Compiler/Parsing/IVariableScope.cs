using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public interface IVariableScope
    {
        IEnumerable<VariableDefinition> GetDefinedVariables(int position);
    }
}
