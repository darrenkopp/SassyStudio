using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public abstract class ImportDirective : ComplexItem
    {
        public AtRule Rule { get; protected set; }
        public TokenItem Semicolon { get; protected set; }

        protected abstract void ParseImport(IItemFactory itemFactory, ITextProvider text, ITokenStream stream);

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, "import"))
            {
                Rule = AtRule.CreateParsed(itemFactory, text, stream);
                if (Rule != null)
                    Children.Add(Rule);

                ParseImport(itemFactory, text, stream);

                if (stream.Current.Type == TokenType.Semicolon)
                    Semicolon = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);
            }

            return Children.Count > 0;
        }
    }
}
