using System;
using System.IO;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio
{
    public class SassDocument : ISassDocument
    {
        readonly object locker = new object();
        public SassDocument(FileInfo source)
        {
            Source = source;
        }

        public event EventHandler<StylesheetChangedEventArgs> StylesheetChanged;

        public FileInfo Source { get; private set; }
        public ISassStylesheet Stylesheet { get; private set; }

        public ISassStylesheet Update(ISassStylesheet stylesheet)
        {
            lock (locker)
            {
                var previous = Stylesheet;
                Stylesheet = stylesheet;

                //DumpTree(stylesheet.Children, 0);
                OnStylesheetChanged(previous, stylesheet);

                return previous;
            }
        }

        private void DumpTree(ParseItemList items, int depth)
        {

            //var indent = new string(' ', depth);
            var indent = "";
            for (int i = 0; i < depth; i++)
            {
                indent += "|  ";
            }

            //indent += "| ";
            foreach (var item in items)
            {
                //if (item is BlockItem)
                //{
                //    var block = item as BlockItem;
                //    if (block.CloseCurlyBrace == null)
                //    {
                //        Logger.Log(string.Format("{0} of {1} on line {2}", block.GetType().Name, block.Parent.GetType().Name, Tree.SourceText.GetLineFromPosition(block.OpenCurlyBrace.Start).LineNumber));
                //    }
                //}

                OutputLogger.Log(string.Format("{0} {1}", indent, item.GetType().Name));
                //string content = string.Empty;
                //if (item is TokenItem && ((item as TokenItem).SourceType == TokenType.String || (item as TokenItem).SourceType == TokenType.BadString))
                //    content = snapshot.GetText(item.Start, item.Length).Replace("\r","\\r").Replace("\n","\\n");

                //if (!string.IsNullOrEmpty(content))
                //    Logger.Log(string.Format("{0} {1} - {2}", indent, item.GetType().Name, content));

                var complex = item as ComplexItem;
                if (complex != null)
                    DumpTree(complex.Children, depth + 1);

                var simplex = item as SimplexItem;
                if (simplex != null)
                    DumpTree(simplex.Children, depth + 1);
            }
        }

        private void OnStylesheetChanged(ISassStylesheet previous, ISassStylesheet current)
        {
            var handler = StylesheetChanged;
            if (handler != null)
                handler(this, new StylesheetChangedEventArgs(previous, current));
        }
    }
}