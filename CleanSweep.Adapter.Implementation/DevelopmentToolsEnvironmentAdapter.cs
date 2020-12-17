namespace CleanSweep.Adapter.Implementation
{
    using EnvDTE;

    using CleanSweep.Adapter.Contract;

    public class DevelopmentToolsEnvironmentAdapter : IDevelopmentToolsEnvironmentAdapter
    {
        private readonly _DTE dte;

        public DevelopmentToolsEnvironmentAdapter(_DTE dte)
        {
            this.dte = dte;

            this.dte.Events.BuildEvents.OnBuildDone += OnBuildDone;
        }

        private void OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            if (action == vsBuildAction.vsBuildActionClean)
            {
                this.OnCleanDone();
            }
        }

        public event EventHandler OnCleanDone;

        public string SolutionFileName
        {
            get
            {
                return this.dte.Solution.FileName;
            }
        }
    }
}