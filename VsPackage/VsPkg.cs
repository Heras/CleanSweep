namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;

    [InstalledProductRegistration("#100", "#102", "1.0.0.0")]
    [Guid(GuidList.guidVSPackagePkgString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideBindingPath()]
    public sealed class VSPackage : Package
    {
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
