namespace CleanSweep.Adapter.Implementation
{
    using EnvDTE;

    using CleanSweep.Adapter.Contract;

    public class DevelopmentToolsEnvironmentAdapter : IDevelopmentToolsEnvironmentAdapter
    {
        private readonly _DTE developmentToolsEnvironment;
        private readonly BuildEvents buildEvents;

        public DevelopmentToolsEnvironmentAdapter(_DTE dte)
        {
            this.developmentToolsEnvironment = dte;
            this.buildEvents = this.developmentToolsEnvironment.Events.BuildEvents;

            this.buildEvents.OnBuildDone += OnBuildDone;
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
                return this.developmentToolsEnvironment.Solution.FileName;
            }
        }
    }
}