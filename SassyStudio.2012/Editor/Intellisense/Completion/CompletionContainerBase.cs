using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    abstract class CompletionContainerBase : IIntellisenseContainer
    {
        protected readonly List<IIntellisenseContainer> Containers = new List<IIntellisenseContainer>(0);
        protected readonly SortedList<int, ICompletionValue> _Variables = new SortedList<int, ICompletionValue>(0);
        protected readonly SortedList<int, ICompletionValue> _Functions = new SortedList<int, ICompletionValue>(0);
        protected readonly SortedList<int, ICompletionValue> _Mixins = new SortedList<int, ICompletionValue>(0);

        public virtual int Start { get; protected set; }
        public virtual int End { get; protected set; }

        public virtual bool IsApplicableTo(int position)
        {
            return position >= Start && position <= End;
        }

        protected virtual void Parse(IIntellisenseContainer container, ParseItemList items, ITextProvider text)
        {
            if (items.Count == 0)
                return;

            foreach (var item in items)
                container.Add(item, text);

            Containers.Add(container);
        }

        public virtual void Add(ParseItem item, ITextProvider text)
        {
            if (item is VariableDefinition)
            {
                AddVariable(item as VariableDefinition, text);
            }
            else if (item is BlockItem)
            {
                var block = item as BlockItem;
                Parse(new BlockScopeContainer(block), block.Children, text);
            }
        }

        IEnumerable<ICompletionValue> TraverseContainers(int position, Func<IIntellisenseContainer, IEnumerable<ICompletionValue>> projection)
        {
            return Containers.Where(x => x.IsApplicableTo(position)).SelectMany(x => projection(x));
        }

        public virtual IEnumerable<ICompletionValue> GetVariables(int position)
        {
            return _Variables.Where(x => x.Key < position).Select(x => x.Value)
                .Concat(TraverseContainers(position, x => x.GetVariables(position)));
        }

        public virtual IEnumerable<ICompletionValue> GetFunctions(int position)
        {
            return _Functions
                .Where(x => x.Key < position).Select(x => x.Value)
                .Concat(TraverseContainers(position, x => x.GetFunctions(position)));
        }

        public virtual IEnumerable<ICompletionValue> GetMixins(int position)
        {
            return _Mixins
                .Where(x => x.Key < position).Select(x => x.Value)
                .Concat(TraverseContainers(position, x => x.GetMixins(position)));
        }

        protected void AddVariable(VariableDefinition variable, ITextProvider text)
        {
            if (variable != null && variable.IsValid)
            {
                var variableName = variable.Name;
                var builder = new StringBuilder(variableName.Length);
                builder.Append(variableName.Prefix.SourceType == TokenType.Dollar ? '$' : '!');
                builder.Append(text.GetText(variableName.Name.Start, variableName.Name.Length));

                var name = builder.ToString();
                _Variables.Add(variable.Start, new VariableCompletionValue
                {
                    DisplayText = name,
                    CompletionText = name,
                    Start = variableName.Start,
                    End = variableName.End,
                    Length = variableName.Length
                });
            }
        }
    }
}
