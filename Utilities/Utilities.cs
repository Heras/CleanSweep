/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using Microsoft.Samples.VisualStudio.CodeSweep.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Samples.VisualStudio.CodeSweep
{
    /// <summary>
    /// General utility methods.
    /// </summary>
    public static class Utilities
    {
        public static bool UnorderedCollectionsAreEqual<T>(ICollection<T> first, ICollection<T> second)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }

            if (first.Count != second.Count)
            {
                return false;
            }

            foreach (T item in first)
            {
                if (!second.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool OrderedCollectionsAreEqual<T>(IList<T> first, IList<T> second)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }

            if (first.Count != second.Count)
            {
                return false;
            }

            for (int i = 0; i < first.Count; ++i)
            {
                if (second.IndexOf(first[i]) != i)
                {
                    return false;
                }
            }

            return true;
        }

        public static string EncodeProgramFilesVar(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            if (path.StartsWith(programFiles, StringComparison.OrdinalIgnoreCase))
            {
                return "$(ProgramFiles)" + path.Substring(programFiles.Length);
            }
            else
            {
                return path;
            }
        }

        public const int RemotingChannel = 9000;

        public static string GetRemotingUri(int procId, bool includeLocalHostPrefix)
        {
            if (includeLocalHostPrefix)
            {
                return string.Format("tcp://localhost:{0}/ScannerHost-{1}", RemotingChannel, procId);
            }
            else
            {
                return string.Format("ScannerHost-{0}", procId);
            }
        }
    }
}
