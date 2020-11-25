using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    [InstalledProductRegistration("#100", "#102", "1.0.0.0")]
    [Guid(GuidList.guidVSPackagePkgString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    //[ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideBindingPath()]
    public sealed class OnCleanSolutionDoneExtension : Package
    {
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