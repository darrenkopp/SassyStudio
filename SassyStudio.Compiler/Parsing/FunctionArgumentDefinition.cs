using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    class FunctionArgumentDefinition : ComplexItem
    {
        public VariableDefinition Variable { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            var variable = new VariableDefinition(ExpresionMode.Argument);
            if (variable.Parse(itemFactory, text, stream))
            {
                Variable = variable;
                Children.Add(variable);
            }

            return Children.Count > 0;
        }
    }
}
