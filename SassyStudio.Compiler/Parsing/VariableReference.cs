using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class VariableReference : ComplexItem
    {
        public VariableName Name { get; protected set; }
        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            var name = new VariableName(SassClassifierType.VariableReference);
            if (name.Parse(itemFactory, text, stream))
            {
                Name = name;
                Children.Add(name);
            }

            return Children.Count > 0;
        }
    }
}
