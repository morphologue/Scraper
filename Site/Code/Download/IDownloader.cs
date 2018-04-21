namespace Scraper.Code.Download
{
    public interface IDownloader
    {
        // Return the HTML at the given URL. If something goes wrong, e.g. the resource cannot be
        // found, a DownloadException is thrown.
        string Download(string url);
    }
}
