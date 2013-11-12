using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    class IntellisenseCache : IIntellisenseCache
    {
        readonly ISassDocument Document;
        readonly IIntellisenseManager IntellisenseManager;
        public IntellisenseCache(ISassDocument document, IIntellisenseManager intellisenseManager)
        {
            Document = document;
            IntellisenseManager = intellisenseManager;
        }

        IIntellisenseContainer Container { get; set; }

        bool ProcessUpdates { get; set; }
        bool UpdatesPending { get; set; }

        public void Update(ISassStylesheet stylesheet, ITextProvider text)
        {
            var container = new StylesheetContainer(IntellisenseManager);
            foreach (var item in stylesheet.Children)
                container.Add(item, text);

            Container = container;
        }

        public IEnumerable<ICompletionValue> GetVariables(int position)
        {
            return Container.GetVariables(position);
        }

        public IEnumerable<ICompletionValue> GetFunctions(int position)
        {
            return Container.GetFunctions(position);
        }

        public IEnumerable<ICompletionValue> GetMixins(int position)
        {
            return Container.GetMixins(position);
        }
    }
}
