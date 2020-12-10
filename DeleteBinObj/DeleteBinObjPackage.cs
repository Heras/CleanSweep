namespace DeleteBinObj
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Threading;
    using EnvDTE;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(DeleteBinObjPackage.PackageGuidString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class DeleteBinObjPackage : AsyncPackage
    {
        /// <summary>
        /// DeleteBinObjPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "7e43adcf-1439-4297-9e1d-8f2e21aee751";

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            this.BuildEvents.OnBuildDone += OnBuildDone;
        }

        void OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            if (action == vsBuildAction.vsBuildActionClean)
            {
                this.DeleteBinOBjFolders(this.DevelopmentToolsEnvironment.Solution);
            }
        }

        private void DeleteBinOBjFolders(Solution solution)
        {
            this.PaneWriteLine("Delete bin & obj folders started...");
            this.PaneWriteLine($"Solution: {solution.FileName}");

            var solutionFilePath = Path.GetDirectoryName(solution.FileName);
            var solutionFileContents = File.ReadAllText(solution.FileName);

            this.GetProjectFilePaths(solutionFileContents, solutionFilePath)
                .ToList()
                .ForEach(this.DeleteBinOBjFolders);

            this.PaneWriteLine($"Delete bin & obj folders finished");
        }

        private void PaneWriteLine(string message)
        {
            this.PaneWrite(message + Environment.NewLine);
        }

        private void PaneWrite(string message)
        {
            this.Pane.OutputString(message);
        }

        IVsOutputWindowPane pane;

        IVsOutputWindowPane Pane
        {
            get
            {
                if (this.pane == null)
                {
                    var outWindow = GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
                    var paneGuid = VSConstants.GUID_BuildOutputWindowPane;
                    IVsOutputWindowPane generalPane;

                    outWindow.GetPane(ref paneGuid, out generalPane);

                    this.pane = generalPane;
                }

                return this.pane;
            }
        }

        private IEnumerable<string> GetProjectFilePaths(string solutionFileContents, string solutionFilePath)
        {
            var projectReferencePattern = "\"(?<project>[^\"]*.csproj)";

            return new Regex(projectReferencePattern)
                .Matches(solutionFileContents)
                .Cast<Match>()
                .Select(m => m.Groups
                    .Cast<Group>()
                    .Last()
                    .Value)
                .Select(f => {
                    this.PaneWriteLine($"Project: {solutionFilePath}\\{f}");
                    return f;
                })
                .Select(f => Path.GetDirectoryName(f))
                .Select(p => $"{solutionFilePath}\\{p}");
        }

        private void DeleteBinOBjFolders(string projectFilePath)
        {
            this.PaneWriteLine($"Project FilePath: {projectFilePath}");

            new[] { "bin", "obj" }
                .ToList()
                .ForEach(f => {
                    Directory.Delete($"{projectFilePath}\\{f}", true);
                    this.PaneWriteLine($"Folder: {f}: Ok");
                });
        }


        private BuildEvents BuildEvents
        {
            get
            {
                if (this.buildEvents == null)
                {
                    this.buildEvents = this.DevelopmentToolsEnvironment.Events.BuildEvents;
                }

                return this.buildEvents;
            }
        }

        private BuildEvents buildEvents;

        private DTE developmentToolsEnvironment;

        private DTE DevelopmentToolsEnvironment
        {
            get
            {
                if (this.developmentToolsEnvironment == null)
                {
                    this.developmentToolsEnvironment = this.GetService(typeof(DTE)) as DTE;
                }

                return this.developmentToolsEnvironment;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    this.buildEvents.OnBuildDone -= OnBuildDone;

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
