namespace CleanSweep.Adapter.Contract
{
    public delegate void EventHandler();

    public interface IDevelopmentToolsEnvironmentAdapter
    {
        string SolutionFileName { get; }

        event EventHandler OnCleanDone;
    }
}