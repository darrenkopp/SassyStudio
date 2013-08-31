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
        protected readonly LinkedList<IIntellisenseContainer> Containers = new LinkedList<IIntellisenseContainer>();
        protected readonly LinkedList<ICompletionValue> Variables = new LinkedList<ICompletionValue>();

        protected virtual void Parse(IIntellisenseContainer container, ParseItemList items, ITextProvider text)
        {
            foreach (var item in items)
                container.Add(item, text);

            Containers.AddLast(container);
        }

        public virtual void Add(ParseItem item, ITextProvider text)
        {
            if (item is VariableDefinition)
            {
                AddVariable(item as VariableDefinition, text);
            }
        }

        public virtual IEnumerable<ICompletionValue> GetVariables(int position)
        {
            return Variables.Where(x => x.End < position)
                .Concat(Containers.SelectMany(x => x.GetVariables(position)));
        }

        public virtual IEnumerable<ICompletionValue> GetFunctions(int position)
        {
            yield break;
        }

        public virtual IEnumerable<ICompletionValue> GetMixins(int position)
        {
            yield break;
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
                Variables.AddLast(new VariableCompletionValue
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
