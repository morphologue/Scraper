using System.Net;

namespace Scraper.Code.Download
{
    // An IDownloader which sends an HTTP GET request to a remote server.
    public class HttpDownloader : IDownloader
    {
        // The is a User-Agent string for Chrome 66.
        const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.117 Safari/537.36";

        // See the comments in IDownloader for this method's contract.
        public string Download(string url)
        {
            using (WebClient http = new WebClient()) {
                // This might make the server happier.
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
