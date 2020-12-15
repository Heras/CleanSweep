namespace DeleteBinObj
{
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;
    using System;

    public class VisualStudioBuildOutputWindowPane : ILogAdapter
    {
        private readonly IVsOutputWindow outputWindow;

        public VisualStudioBuildOutputWindowPane(IVsOutputWindow outputWindow)
        {
            this.outputWindow = outputWindow;
        }

        public void WriteLine(string message)
        {
            this.Pane.OutputString(message + Environment.NewLine);
        }

        IVsOutputWindowPane Pane
        {
            get
            {
                var paneGuid = VSConstants.GUID_BuildOutputWindowPane;
                IVsOutputWindowPane generalPane;
                this.outputWindow.GetPane(ref paneGuid, out generalPane);
                return generalPane;
            }
        }
    }
}