using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using EnvDTE;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    [InstalledProductRegistration("#100", "#102", "1.0.0.0")]
    [Guid(guidVSPackagePkgString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    [ProvideBindingPath()]
    public sealed class OnCleanSolutionDoneExtension : Package
    {
        public const string guidVSPackagePkgString = "2b621c1e-60a3-48c5-a07d-0ad6d3dd3417";
        public static readonly Guid guidVSPackagePkg = new Guid(guidVSPackagePkgString);
        public static readonly Guid guidVSPackageCmdSet = new Guid("d0882566-3d01-4578-b4f2-0aff36119700");

        protected override void Initialize()
        {
            base.Initialize();

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