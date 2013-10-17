using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor
{
    interface IRootLevelDocumentCache
    {
        void Initialize();

        IReadOnlyCollection<ISassDocument> Documents { get; }
    }

    [Export(typeof(IRootLevelDocumentCache))]
    class RootLevelDocumentCache : IRootLevelDocumentCache
    {
        readonly ConcurrentBag<ISassDocument> _Documents = new ConcurrentBag<ISassDocument>();
        readonly IDocumentManager DocumentManager;

        [ImportingConstructor]
        public RootLevelDocumentCache(IDocumentManager documentManager)
        {
            DocumentManager = documentManager;

            Initialize();
        }

        public IReadOnlyCollection<ISassDocument> Documents { get { return new List<ISassDocument>(_Documents); } } 

        public void Initialize()
        {
            Task.Run(() =>
            {
                foreach (var file in Flatten().Where(IsRootLevelDocument))
                    _Documents.Add(DocumentManager.Get(file));
            });
        }

        public IEnumerable<FileInfo> Flatten()
        {
            var solution = SassyStudioPackage.Instance.DTE.Solution;

            return solution.Projects.OfType<EnvDTE.Project>()
                .SelectMany(x => Flatten(x.ProjectItems));
        }

        private IEnumerable<FileInfo> Flatten(EnvDTE.ProjectItems items)
        {
            foreach (EnvDTE.ProjectItem item in items)
            {
                if (item.FileCount > 0)
                    yield return new FileInfo(item.Properties.Item("FullPath").Value.ToString());

                foreach (var child in Flatten(item.ProjectItems))
                    yield return child;
            }
        }

        static bool IsRootLevelDocument(FileInfo file)
        {
            return file.Extension == ".scss" && !file.Name.StartsWith("_");
        }
    }
}
