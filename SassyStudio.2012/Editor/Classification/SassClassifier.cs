using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Classification
{
    class SassClassifier : IClassifier
    {
        readonly ITextBuffer Buffer;
        readonly IClassificationTypeRegistryService Registry;
        readonly ISassEditor Editor;

        public SassClassifier(ITextBuffer buffer, ISassEditor editor, IClassificationTypeRegistryService registry)
        {
            Buffer = buffer;
            Registry = registry;
            Editor = editor;

            Editor.DocumentChanged += OnDocumentChanged;
        }        
        
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            var handler = ClassificationChanged;
            if (handler != null)
                handler(this, new ClassificationChangedEventArgs(new SnapshotSpan(Buffer.CurrentSnapshot, e.ChangeStart, e.ChangeEnd - e.ChangeStart)));
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var results = new List<ClassificationSpan>();
            var snapshot = Buffer.CurrentSnapshot;
            var stylesheet = Editor.Document.Stylesheet;

            if (stylesheet != null)
            {
                try
                {
                    var item = stylesheet.Children.FindItemContainingPosition(span.Start);
                    if (item == null)
                        item = stylesheet as Stylesheet;

                    if (!(item is IParseItemContainer))
                        item = item.Parent ?? item;

                    for (var current = item; current != null; current = current.InOrderSuccessor())
                    {
                        if (current.Start <= span.End && current.End >= span.Start)
                        {
                            if (current.Start > span.End)
                                break;

                            var type = ClassifierContextCache.Get(current.ClassifierType).GetClassification(Registry);
                            if (type != null)
                            {
                                var start = Math.Max(0, current.Start);
                                var length = Math.Min(snapshot.Length - start, current.Length);
                                results.Add(new ClassificationSpan(new SnapshotSpan(snapshot, new Span(start, length)), type));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, "Failed to classify");
                }
            }

            return results;
        }
    }
}
