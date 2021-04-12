namespace CleanSweep.Application.Vsix
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    using EnvDTE;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    using Task = System.Threading.Tasks.Task;

    using CleanSweep.Adapter.Implementation;

    using CleanSweep.Domain.Contract;
    using CleanSweep.Domain.Implementation;

    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid("7e43adcf-1439-4297-9e1d-8f2e21aee751")]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class CleanSweepPackage : AsyncPackage
    {
        private ICleanSweepService cleanSweepService;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var dte = await this.GetServiceAsync<DTE, _DTE>();
            var developmentToolsEnvironment = new DevelopmentToolsEnvironmentAdapter(dte);

            var outputWindow = GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            var log = new VisualStudioBuildOutputWindowPane(outputWindow);

            var fileSystem = new WindowsFileSystem();

            this.cleanSweepService = new CleanSweepService(developmentToolsEnvironment, log, fileSystem);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    this.cleanSweepService.Dispose();

                    GC.SuppressFinalize(this);
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}