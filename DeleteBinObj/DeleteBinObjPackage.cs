namespace DeleteBinObj
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
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
    [Guid("7e43adcf-1439-4297-9e1d-8f2e21aee751")]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class DeleteBinObjPackage : AsyncPackage
    {
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
                this.DeleteBinOBjFolders(this.DevelopmentToolsEnvironment.Solution.FileName);
            }
        }

        private void DeleteBinOBjFolders(string solutionFileName)
        {
            this.PaneWriteLine("Delete bin & obj folders started...");

            var solutionFileContents = File.ReadAllText(solutionFileName);
            var solutionFilePath = Path.GetDirectoryName(solutionFileName);

            var results = this.GetProjectFilePaths(solutionFileContents, solutionFilePath)
                .SelectMany((p, i) => this.DeleteBinOBjFolders(p.ProjectName, p.ProjectFilePath, i));
 
            this.PaneWriteLine($"========== Delete bin & obj: {results.Count(c => c == HttpStatusCode.NoContent)} succeeded, {results.Count(c => c == HttpStatusCode.InternalServerError)} failed, {results.Count(c => c == HttpStatusCode.NotFound)} skipped ==========");
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

        private IEnumerable<(string ProjectName, string ProjectFilePath)> GetProjectFilePaths(string solutionFileContents, string solutionFilePath)
        {
            var projectReferencePattern = "\"(?<project>[^\"]*).csproj";

            return new Regex(projectReferencePattern)
                .Matches(solutionFileContents)
                .Cast<Match>()
                .Select(m => m.Groups
                    .Cast<Group>()
                    .Last()
                    .Value)
                .Select(n => (n, Path.GetDirectoryName(n + ".csproj")))
                .Select(t => (t.n, $"{solutionFilePath}\\{t.Item2}"));
        }

        private IEnumerable<HttpStatusCode> DeleteBinOBjFolders(string projectName, string projectFilePath, int index)
        {
            this.PaneWriteLine($"{index+1}>------ Delete bin & obj folders started: Project: {projectName}");

            return new[] { "bin", "obj" }
                .Select(f => $"{projectFilePath}\\{f}")
                .Select(p => this.Delete(p, index));
        }

        private HttpStatusCode Delete(string path, int index)
        {
            this.PaneWriteLine($"{index+1}>------ Delete {path}");

            if (false == Directory.Exists(path))
            {
                return HttpStatusCode.NotFound;
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception e)
            {
                this.PaneWriteLine(e.Message);

                return HttpStatusCode.InternalServerError;
            }

            return HttpStatusCode.NoContent;
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
