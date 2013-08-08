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
        public virtual bool IsValid { get { return true; } }

        public abstract bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream);
        public virtual void Freeze()
        {
        }

        public ComplexItem Parent { get; set; }

        public ParseItem InOrderSuccessor()
        {
            var container = this as ComplexItem;
            if (container != null && container.Children.Count > 0)
                return container.Children.First();

            return NextSibling();
        }

        public ParseItem InOrderPredecessor()
        {
            var sibling = PreviousSibling();
            if (sibling != null)
            {
                var container = sibling as ComplexItem;
                if (container != null)
                    return container.Children[container.Children.Count - 1];
            }

            return Parent;
        }

        public ParseItem NextSibling()
        {
            if (Parent == null) return null;

            // get next child after this one
            var siblingIndex = Parent.Children.IndexOf(this) + 1;
            if (siblingIndex < Parent.Children.Count)
                return Parent.Children[siblingIndex];

            // if this is last of child of parent, then get parents next sibling
            return Parent.NextSibling();
        }

        public ParseItem PreviousSibling()
        {
            if (Parent == null) return null;

            var siblingIndex = Parent.Children.IndexOf(this) - 1;
            if (siblingIndex >= 0)
                return Parent.Children[siblingIndex];

            return null;
        }
    }
}
