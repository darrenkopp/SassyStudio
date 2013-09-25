using System;
using System.ComponentModel.Composition.Hosting;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SassyStudio.Options;

namespace SassyStudio
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "0.7.3", IconResourceID = 400)]
    [GuidAttribute(Guids.guidSassyStudioPkgString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideOptionPage(typeof(ScssOptions), "Sassy Studio", "General", 101, 102, true, new string[] { "CSS", "SCSS" })]
    public sealed class SassyStudioPackage : Package
    {
        readonly CancellationTokenSource CancellationTokens = new CancellationTokenSource();
        readonly Lazy<ScssOptions> _ScssOptions;
        readonly OptionsProvider _Options;
        readonly Lazy<DTE2> _DTE;
        public SassyStudioPackage()
        {
            _ScssOptions = new Lazy<ScssOptions>(() => GetDialogPage(typeof(ScssOptions)) as ScssOptions, true);
            _Options = new OptionsProvider(this);
            _DTE = new Lazy<DTE2>(() => ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2);
        }

        internal static SassyStudioPackage Instance { get; private set; }
        public CancellationToken ShutdownToken { get { return CancellationTokens.Token; } }
        protected override void Initialize()
        {
            base.Initialize();
            Instance = this;

            Composition = InitializeComposition();
            OutputLogger.MessageReceived += (s, e) => Logger.Log(e.Message);
            OutputLogger.ExceptionReceived += (s, e) => Logger.Log(e.Error, e.Message);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                using (CancellationTokens)
                    CancellationTokens.Cancel();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        internal CompositionContainer Composition { get; private set; }
        internal OptionsProvider Options { get { return _Options; } }
        internal DTE2 DTE { get { return _DTE.Value; } }
        public int LocaleId
        {
            get
            {
                IUIHostLocale globalService = (IUIHostLocale)GetGlobalService(typeof(IUIHostLocale));
                uint localeId;
                if (globalService != null && globalService.GetUILocale(out localeId) == 0)
                    return (int)localeId;

                return 0;
            }
        }

        private CompositionContainer InitializeComposition()
        {
            var catalog = new AggregateCatalog(
                new AssemblyCatalog(typeof(SassyStudioPackage).Assembly),
                new AssemblyCatalog(typeof(TokenType).Assembly),
                new AssemblyCatalog(typeof(ISassDocument).Assembly)
            );

            return new CompositionContainer(catalog);
        }

        internal class OptionsProvider
        {
            readonly SassyStudioPackage _Package;
            public OptionsProvider(SassyStudioPackage package)
            {
                _Package = package;
            }

            public ScssOptions Scss { get { return _Package._ScssOptions.Value; } }
        }
    }
}
