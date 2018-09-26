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
            var fileInfo = new FileInfo(destination);
            if (!fileInfo.Exists)
            {
                using (var wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "Rhetos CLI");
                    wc.DownloadFileCompleted += HandleDownloadComplete;
                    var syncObject = new Object();
                    lock (syncObject)
                    {
                        Logging.LogInfo("Downloading file {0} {1}%", Url, 0);
                        wc.DownloadFileAsync(uri, destination, syncObject);
                        //Wait for download to complete (or fail)...
                        Monitor.Wait(syncObject);
                    }
                }
            }
            else
            {
                Logging.LogWarn("File exists download skipped...");
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
                    Logging.LogInfo("Download completed");
                }
                else
                {
                    Logging.LogError(e.Error, "There was error downloading file {0}", Url);
                }
                //releases blocked thread
                Monitor.Pulse(e.UserState);
            }
        }
    }
}