using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class ParseItemList : IList<ParseItem>
    {
        readonly List<ParseItem> Inner = new List<ParseItem>(0);
        public ParseItemList()
        {
        }

        public ComplexItem AutoParent { get; set; }

        public void TrimExcess()
        {
            Inner.TrimExcess();
        }

        public TokenItem AddCurrentAndAdvance(ITokenStream stream, SassClassifierType classifierType = SassClassifierType.Default)
        {
            var item = new TokenItem(stream.Current, classifierType);
            Add(item);

            stream.Advance();
            return item;
        }

        public ParseItem FindItemContainingPosition(int position)
        {
            int start = 0;
            int end = Count - 1;

            ParseItem match = null;
            while (start <= end)
            {
                int midpoint = (start + end) / 2;
                var current = this[midpoint];

                // if child contains position, we can stop
                if (current.Start <= position && current.End >= position)
                {
                    match = current;
                    break;
                }

                if (position > current.End)
                    start = midpoint + 1;

                if (position < current.Start)
                    end = midpoint - 1;
            }

            if (match != null)
            {
                // if match is container, search children for smaller container
                var container = match as ComplexItem;
                if (container != null)
                    return container.Children.FindItemContainingPosition(position) ?? match;

                // if match was not a complex item (ie token item) then return it's parent
                return match;
            }

            return null;
        }

        public int IndexOf(ParseItem item)
        {
            return Inner.IndexOf(item);
        }

        public void Insert(int index, ParseItem item)
        {
            if (AutoParent != null)
                item.Parent = AutoParent;

            Inner.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Inner.RemoveAt(index);
        }

        public ParseItem this[int index]
        {
            get
            {
                return Inner[index];
            }
            set
            {
                Inner[index] = value;
            }
        }

        public void Add(ParseItem item)
        {
            if (AutoParent != null)
                item.Parent = AutoParent;

            Inner.Add(item);
        }

        public void Clear()
        {
            Inner.Clear();
        }

        public bool Contains(ParseItem item)
        {
            return Inner.Contains(item);
        }

        public void CopyTo(ParseItem[] array, int arrayIndex)
        {
            Inner.CopyTo(array, arrayIndex);
        }

        public int Count { get { return Inner.Count; } }

        public bool IsReadOnly { get { return false; } }

        public bool Remove(ParseItem item)
        {
            return Inner.Remove(item);
        }

        public IEnumerator<ParseItem> GetEnumerator()
        {
            return Inner.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Inner.GetEnumerator();
        }
    }
}
