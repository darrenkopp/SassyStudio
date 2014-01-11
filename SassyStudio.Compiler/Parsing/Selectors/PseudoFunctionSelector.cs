using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing.Selectors
{
    public class PseudoFunctionSelector : SimpleSelector
    {
        public TokenItem Prefix { get; protected set; }
        public Function Function { get; protected set; }

        protected override bool ParseSelectorToken(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Colon && stream.Peek(1).Type == TokenType.Function)
            {
                Prefix = Children.AddCurrentAndAdvance(stream);
                var function = itemFactory.CreateSpecific<PseduoFunction>(this, text, stream);
                if (function.Parse(itemFactory, text, stream))
                {
                    Function = function;
                    Children.Add(function);
                }
            }

            return Children.Count > 0;
        }
    }
}
