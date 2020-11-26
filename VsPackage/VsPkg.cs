using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    [InstalledProductRegistration("#100", "#102", "1.0.0.0")]
    [Guid(GuidList.guidVSPackagePkgString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideBindingPath()]
    public sealed class VSPackage : Package
    {
        private readonly IChannel _tcpChannel = new TcpChannel(Utilities.RemotingChannel);

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
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
