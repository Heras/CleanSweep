namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;

    [InstalledProductRegistration(
        "productName",
        "productDetails",
        "1.0.0.0")]
    [Guid(guidVSPackagePkgString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideBindingPath()]
    public sealed class VSPackage : Package
    {
        public const string guidVSPackagePkgString = "2b621c1e-60a3-48c5-a07d-0ad6d3dd3417";

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
