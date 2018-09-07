﻿using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Compression;
using System.IO;

namespace RhetosCLI.Commands
{
    public static class Releases
    {
        public static void ListReleases()
        {
            //Returns list of rhetos relases in string
            //ready for Console/File output
            var result = new List<string>();
            var releases = RhetosGitGubCllient.GetAllReleases().Result.OrderByDescending(f => f.PublishedAt).ToList();

            foreach (var release in releases)
            {
                result.Add(string.Format("Version: {0}, Published at: {1}", release.TagName, release.PublishedAt.ToString()));
            }
            result.ForEach(Helpers.WriteLine);
        }

        public static string DownloadRhetosRelease(string version)
        {
            //Returns zip for release
            var extractedPath = "";
            var release = GetReleaseData(version);

            if (release != null)
            {
                var archivePath = Helpers.DownloadRelease(release.Assets.First().BrowserDownloadUrl);
                extractedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileNameWithoutExtension(archivePath));
                Helpers.WriteLine("Extracting {0} to {1}", archivePath, extractedPath);
                Helpers.UnzipFile(archivePath, extractedPath);
                Helpers.WriteLine("Extraction done", ConsoleColor.Green);
            }
            else
            {
                Helpers.WriteLine("ERROR! Can't find version {0}", version, ConsoleColor.Red);
            }
            return extractedPath;
        }

        private static Release GetReleaseData(string version)
        {
            //first looking for release by name then tag
            var releases = RhetosGitGubCllient.GetAllReleases().Result.OrderByDescending(f => f.PublishedAt).ToList();
            var release = releases.SingleOrDefault(r => r.Name == version);

            if (release == null)
            {
                release = releases.SingleOrDefault(r => r.TagName == version);
            }

            return release;
        }
    }
}