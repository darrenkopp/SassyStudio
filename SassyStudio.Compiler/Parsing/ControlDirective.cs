using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public abstract class ControlDirective : ComplexItem
    {
        public AtRule Rule { get; protected set; }
        public ControlDirectiveBody Body { get; protected set; }

        public virtual bool ParseRule(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            Rule = AtRule.CreateParsed(itemFactory, text, stream);
            if (Rule != null)
                Children.Add(Rule);

            return Rule != null;
        }

        public virtual bool ParseBody(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            var body = new ControlDirectiveBody();
            if (body.Parse(itemFactory, text, stream))
            {
                Body = body;
                Children.Add(Body);
                return true;
            }

            return false;
        }
    }
}
