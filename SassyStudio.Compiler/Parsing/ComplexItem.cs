using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public abstract class ComplexItem : ParseItem, IParseItemContainer, IVariableScope
    {
        public ComplexItem()
        {
            Children = new ParseItemList();
            Children.AutoParent = this;
        }

        public ParseItemList Children { get; protected set; }

        public override int Start { get { return (Children.Count == 0) ? 0 : Children[0].Start; } }

        public override int End { get { return (Children.Count == 0) ? 0 : Children[Children.Count - 1].End; } }

        public override int Length { get { return End - Start; } }

        public override void Freeze()
        {
            base.Freeze();
            Children.TrimExcess();

            for (int i = 0; i < Children.Count; i++)
                Children[i].Freeze();
        }

        public virtual IEnumerable<VariableName> GetDefinedVariables(int position)
        {
            foreach (var variable in Children.Where(x => x.Start < position).OfType<VariableDefinition>().Select(x => x.Name))
                yield return variable;
        }

    }
}
