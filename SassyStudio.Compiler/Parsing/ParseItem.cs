using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public abstract class ParseItem : IRange
    {
        public SassClassifierType ClassifierType { get; protected set; }
        //public TokenType TokenType { get; protected set; }
        public abstract int Start { get; }
        public abstract int End { get; }
        public abstract int Length { get; }

        public abstract bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream);
        public virtual void Freeze()
        {
        }
    }
}
