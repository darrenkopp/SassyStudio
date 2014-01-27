using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SassyStudio.Editor;

namespace SassyStudio.Compiler.Parsing
{
    public class VariableReference : ComplexItem, IResolvableToken
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

        public ParseItem GetSourceToken()
        {
            return ReverseSearch.Find<VariableDefinition>(this, x => x.Name.Equals(Name));
        }
    }
}
