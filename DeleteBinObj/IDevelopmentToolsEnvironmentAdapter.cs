namespace DeleteBinObj
{
    using EnvDTE;

    public delegate void EventHandler();

    public interface IDevelopmentToolsEnvironmentAdapter
    {
        string SolutionFileName { get; }

        event EventHandler OnCleanDone;
    }

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