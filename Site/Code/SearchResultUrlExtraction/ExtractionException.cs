using System;

namespace Scraper.Code.SearchResultUrlExtraction
{
    // Thrown when there is a somewhat expected problem with extracting URLs, e.g. Google shows a
    // reCAPTCHA
    public class ExtractionException : Exception
    {
        public ExtractionException(string message) : base(message) { }

        public ExtractionException(string message, Exception inner) : base(message, inner) { }
    }
}
