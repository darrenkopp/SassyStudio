using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class NamedFunctionArgument : FunctionArgument
    {
        public VariableReference Variable { get; protected set; }
        public TokenItem Colon { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (IsNamedArgument(text, stream))
            {
                var variable = itemFactory.CreateSpecific<VariableReference>(this, text, stream);
                if (variable.Parse(itemFactory, text, stream))
                {
                    Variable = variable;
                    Children.Add(variable);
                }

                if (stream.Current.Type == TokenType.Colon)
                    Colon = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);

                base.Parse(itemFactory, text, stream);
                return true;
            }

            return false;
        }

        public static bool IsNamedArgument(ITextProvider text, ITokenStream stream)
        {
            return VariableName.IsVariable(text, stream) && stream.Peek(2).Type == TokenType.Colon;
        }
    }
}
