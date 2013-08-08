using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class FunctionArgumentDefinition : ComplexItem
    {
        public VariableDefinition Variable { get; protected set; }

        public TokenItem Comma { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            var variable = new VariableDefinition(ExpresionMode.Argument);
            if (variable.Parse(itemFactory, text, stream))
            {
                Variable = variable;
                Children.Add(variable);
            }

            if (stream.Current.Type == TokenType.Comma)
                Comma = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);

            return Children.Count > 0;
        }
    }
}
