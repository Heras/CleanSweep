/***************************************************************************
 
Copyright (c) Microsoft Corporation. All rights reserved.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using EnvDTE;
using Microsoft.Samples.VisualStudio.CodeSweep.VSPackage.Properties;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    class NonMSBuildProjectConfigStore : IProjectConfigurationStore
    {
        public ICollection<string> TermTableFiles
        {
            get { return _termTableFiles; }
        }

        public ICollection<BuildTask.IIgnoreInstance> IgnoreInstances
        {
            get { return _ignoreInstances; }
        }

        public bool RunWithBuild
        {
            get { return false; }
            set { throw new InvalidOperationException(Resources.RunWithBuildForNonMSBuild); }
        }

        readonly CollectionWithEvents<string> _termTableFiles = new CollectionWithEvents<string>();
        readonly CollectionWithEvents<BuildTask.IIgnoreInstance> _ignoreInstances = new CollectionWithEvents<BuildTask.IIgnoreInstance>();
    }
}
