using System;

namespace Scraper.Code.Download
{
    // Thrown when something goes wrong during a download, e.g. the specified resource cannot be
    // found
    public class DownloadException : Exception
    {
        public DownloadException(string message) : base(message) { }

        public DownloadException(string message, Exception inner) : base(message, inner) { }
    }
}
