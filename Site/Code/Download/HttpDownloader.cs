using System;
using System.Net;
using System.Threading;

namespace Scraper.Code.Download
{
    // An IDownloader which sends an HTTP GET request to a remote server. It pulls a few tricks in
    // order to not let on that we're a bot, specifically: 1) a Chrome user-agent is supplied, and
    // 2) a random delay of between 1 and 3 seconds is introduced between each request.
    public class HttpDownloader : IDownloader
    {
        // The is a User-Agent string for Chrome 66.
        const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.117 Safari/537.36";

        static Random random = new Random();

        bool first_request = true;

        // See the comments in IDownloader for this method's contract.
        public string Download(string url)
        {
            if (!first_request)
                // Sleep for between 1 and 3 seconds between requests.
                Thread.Sleep(random.Next(1000, 3000));
            first_request = false;

            using (WebClient http = new WebClient()) {
                http.Headers.Add("User-Agent", USER_AGENT);

                string result;
                try {
                    result = http.DownloadString(url);
                } catch (WebException ex) {
                    throw new DownloadException($"The resource '{url}' could not be downloaded: {ex.Message}", ex);
                }
                return result;
            }
        }
    }
}
