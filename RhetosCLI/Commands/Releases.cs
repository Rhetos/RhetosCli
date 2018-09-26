using Octokit;
using RhetosCLI.Attributes;
using RhetosCLI.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RhetosCLI.Commands
{
    [ClicommandModuleAttribute("Release", "Get list of releases or download specific version from github")]
    public class Releases
    {
        public string RhetosVersion { get; set; }

        public Releases()
        {
        }

        public Releases(string version)
        {
            RhetosVersion = version;
        }

        [CliCommand("List ", "List all rhetos releases")]
        public void ListReleases()
        {
            var result = new List<string>();
            var releases = RhetosGitGubCllient.GetAllReleases().Result.OrderByDescending(f => f.PublishedAt).ToList();

            foreach (var release in releases)
            {
                result.Add(string.Format("Version: {0}, Published at: {1}", release.TagName, release.PublishedAt.ToString()));
            }
            foreach (var res in result)
            {
                Logging.LogInfo(res);
            }
        }

        [CliCommand("Get", "Downloads specific rhetos version (use 'latest' for last published")]
        public string DownloadRhetosRelease(string version)
        {
            var extractedPath = "";
            var release = GetReleaseData(version);

            if (release != null)
            {
                var archivePath = MiscHelpers.DownloadRelease(release.Assets.First().BrowserDownloadUrl);
                extractedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(archivePath));
                Logging.LogInfo("Extracting {0} to {1}", archivePath, extractedPath);
                MiscHelpers.UnzipFile(archivePath, extractedPath);
                Logging.LogInfo("Extraction done");
            }
            else
            {
                throw new ArgumentException(string.Format("Can't find version {0}", version));
            }
            return extractedPath;
        }

        private Release GetReleaseData(string version)
        {
            //first looking for release by name then tag
            Release release = null;
            var releases = RhetosGitGubCllient.GetAllReleases().Result.OrderByDescending(f => f.PublishedAt).ToList();
            if (string.Equals(version, "latest", StringComparison.OrdinalIgnoreCase))
            {
                release = releases.First();
            }
            else
            {
                release = releases.SingleOrDefault(r => r.Name == version);
            }
            if (release == null)
            {
                release = releases.SingleOrDefault(r => r.TagName == version);
            }
            return release;
        }

        [CliCommand("Help", "Shows help for module commands")]
        public void ShowHelp(CliCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}