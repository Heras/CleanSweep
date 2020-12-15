using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace DeleteBinObj
{
    internal class CleanSweepService : IDisposable, ICleanSweepService
    {
        private readonly IDevelopmentToolsEnvironmentAdapter developmentToolsEnvironment;
        private readonly ILogAdapter log;
        private readonly IFileSystemAdapter fileSystem;

        public CleanSweepService(IDevelopmentToolsEnvironmentAdapter developmentToolsEnvironment, ILogAdapter log, IFileSystemAdapter fileSystem)
        {
            this.developmentToolsEnvironment = developmentToolsEnvironment;
            this.log = log;
            this.fileSystem = fileSystem;
        }

        public void WireUp()
        {
            this.developmentToolsEnvironment.OnCleanDone += DeleteBinObjFolders;
        }

        private void DeleteBinObjFolders()
        {
            this.log.WriteLine("Delete bin & obj folders started...");

            var solutionFileContents = this.fileSystem.ReadAllFileText(this.developmentToolsEnvironment.SolutionFileName);
            var solutionFilePath = this.fileSystem.GetDirectoryName(this.developmentToolsEnvironment.SolutionFileName);

            var results = this.GetProjectFilePaths(solutionFileContents, solutionFilePath)
                .SelectMany((p, i) => this.DeleteBinOBjFolders(p.ProjectName, p.ProjectFilePath, i));

            this.log.WriteLine($"========== Delete bin & obj: {results.Count(c => c == HttpStatusCode.NoContent)} succeeded, {results.Count(c => c == HttpStatusCode.InternalServerError)} failed, {results.Count(c => c == HttpStatusCode.NotFound)} skipped ==========");
        }

        private IEnumerable<(string ProjectName, string ProjectFilePath)> GetProjectFilePaths(string solutionFileContents, string solutionFilePath)
        {
            var projectReferencePattern = "\"(?<project>[^\"]*).csproj";

            return new Regex(projectReferencePattern)
                .Matches(solutionFileContents)
                .Cast<Match>()
                .Select(m => m.Groups
                    .Cast<Group>()
                    .Last()
                    .Value)
                .Select(n => (n, this.fileSystem.GetDirectoryName(n + ".csproj")))
                .Select(t => (t.n, $"{solutionFilePath}\\{t.Item2}"));
        }

        private IEnumerable<HttpStatusCode> DeleteBinOBjFolders(string projectName, string projectFilePath, int index)
        {
            this.log.WriteLine($"{index + 1}>------ Delete bin & obj folders started: Project: {projectName}");

            return new[] { "bin", "obj" }
                .Select(f => $"{projectFilePath}\\{f}")
                .Select(p => this.Delete(p, index));
        }

        private HttpStatusCode Delete(string path, int index)
        {
            this.log.WriteLine($"{index + 1}>------ Delete {path}");

            if (false == this.fileSystem.DirectoryExists(path))
            {
                return HttpStatusCode.NotFound;
            }

            try
            {
                this.fileSystem.DeleteDirectory(path);
            }
            catch (Exception e)
            {
                this.log.WriteLine(e.Message);

                return HttpStatusCode.InternalServerError;
            }

            return HttpStatusCode.NoContent;
        }

        public void Dispose()
        {
            this.developmentToolsEnvironment.OnCleanDone -= DeleteBinObjFolders;
        }
    }
}