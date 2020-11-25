using System;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    // MUST match VSPackage.vsct
    static class GuidList
    {
        public const string guidVSPackagePkgString = "2b621c1e-60a3-48c5-a07d-0ad6d3dd3417";
        public static readonly Guid guidVSPackagePkg = new Guid(guidVSPackagePkgString);
        public static readonly Guid guidVSPackageCmdSet = new Guid("d0882566-3d01-4578-b4f2-0aff36119700");
    }
}
