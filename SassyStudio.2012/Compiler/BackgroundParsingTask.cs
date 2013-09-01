using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SassyStudio.Compiler
{
    [Export(typeof(IBackgroundParsingTask))]
    class BackgroundParsingTask : IBackgroundParsingTask
    {
        readonly IDocumentParserFactory ParserFactory;
        readonly CancellationToken ShutdownToken;
        readonly BlockingCollection<BackgroundParseRequest> Requests = new BlockingCollection<BackgroundParseRequest>();

        [ImportingConstructor]
        public BackgroundParsingTask(IDocumentParserFactory parserFactory, IDocumentManager documentManager)
        {
            ParserFactory = parserFactory;
            ShutdownToken = SassyStudioPackage.Instance.ShutdownToken;

            documentManager.DocumentAdded += OnDocumentAdded;
        }

        public void Start()
        {
            Task.Factory.StartNew(() => ProcessRequests(), ShutdownToken, TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        void OnDocumentAdded(object sender, DocumentAddedEventArgs e)
        {
            Requests.Add(new BackgroundParseRequest(e.Document));
        }

        void ProcessRequests()
        {
            while (!ShutdownToken.IsCancellationRequested)
            {
                BackgroundParseRequest request;
                if (Requests.TryTake(out request, -1, ShutdownToken))
                {
                    try
                    {
                        var source = request.Document.Source;
                        source.Refresh();
                        if (source.Exists)
                        {
                            Logger.Log(string.Format("Background Parse: {0}", source.FullName));

                            ISassStylesheet stylesheet = null;
                            var textManager = new FileTextManager(source);
                            using (var scope = textManager.Open())
                            {
                                var parser = ParserFactory.Create();
                                stylesheet = parser.Parse(new FileParsingRequest(scope.Text, request.Document));
                            }

                            if (stylesheet != null)
                                request.Document.Update(stylesheet);

                            Logger.Log(string.Format("Background Parse Complete: {0}", source.FullName));
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex, "Failed to process background document parse request.");
                    }
                }
            }
        }

        class FileParsingRequest : IParsingRequest
        {
            public FileParsingRequest(ITextProvider provider, ISassDocument document)
            {
                Text = provider;
                Document = document;
            }

            public ISassDocument Document { get; set; }
            public ITextProvider Text { get; private set; }
            public DateTime RequestedOn { get; private set; }
            public bool IsCancelled { get { return false; } }
        }

        class BackgroundParseRequest
        {
            private ISassDocument _Document;

            public BackgroundParseRequest(ISassDocument document)
            {
                _Document = document;
            }

            public ISassDocument Document { get { return _Document; } }
        }
    }
}
