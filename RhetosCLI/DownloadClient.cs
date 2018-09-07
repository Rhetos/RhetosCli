using RhetosCLI.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace RhetosCLI
{
    public class DownloadClient
    {
        public string Url { get; set; }
        public bool ProgressDone { get; set; }

        public DownloadClient(string url)
        {
            this.Url = url;
        }

        public string DownloadFile()
        {
            var uri = new Uri(Url);
            var destination = string.Format("{0}\\{1}", MiscHelpers.GetCachePath(), uri.Segments.Last());
            //TODO: Check for if file exists....and skip download
            var fileInfo = new FileInfo(destination);
            if (!fileInfo.Exists)
            {
                using (var wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "Rhetos CLI");
                    wc.DownloadProgressChanged += HandleDownloadProgress;
                    wc.DownloadFileCompleted += HandleDownloadComplete;
                    var syncObject = new Object();
                    lock (syncObject)
                    {
                        MiscHelpers.WriteLine("Downloading file {0} {1}%", Url, 0);
                        wc.DownloadFileAsync(uri, destination, syncObject);
                        //Wait for download to complete (or fail)...
                        Monitor.Wait(syncObject);
                    }
                }
            }
            else
            {
                MiscHelpers.WriteLine("File exists download skipped...", ConsoleColor.Yellow);
            }
            return destination;
        }

        public void HandleDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            lock (e.UserState)
            {
                if (e.Error == null)
                {
                    ProgressDone = true;
                    MiscHelpers.WriteLine("Download completed", ConsoleColor.Green);
                }
                else
                {
                    MiscHelpers.WriteLine("There was error downloading file", ConsoleColor.Red);
                    MiscHelpers.WriteLine("Exception details: {1}", ConsoleColor.Red, Url, e.Error.ToString());
                }
                //releases blocked thread
                Monitor.Pulse(e.UserState);
            }
        }

        public void HandleDownloadProgress(object sender, DownloadProgressChangedEventArgs args)
        {
            lock (args.UserState)
            {
                if (!ProgressDone)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    MiscHelpers.WriteLine("Downloading file {0} {1}/{2}MB {3}%", Url, args.BytesReceived / 1024, args.TotalBytesToReceive / 1024, args.ProgressPercentage);
                }
            }
        }
    }
}