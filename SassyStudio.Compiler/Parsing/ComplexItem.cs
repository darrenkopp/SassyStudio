using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public abstract class ComplexItem : ParseItem
    {
        public ComplexItem()
        {
            Children = new ParseItemList();
        }

        public ParseItemList Children { get; protected set; }

        public override int Start { get { return (Children.Count == 0) ? 0 : Children[0].Start; } }

        public override int End { get { return (Children.Count == 0) ? 0 : Children[Children.Count - 1].End; } }

        public override int Length { get { return End - Start; } }

        public override void Freeze()
        {
            base.Freeze();

            if (Children.Count > 0)
            {
                foreach (var child in Children)
                    child.Freeze();

                // trim
                Children.TrimExcess();
            }
        }
    }
}
