using System.Collections.Generic;
using System.Net;
using System.Web;
using Scraper.Code.Download;

namespace Scraper.Code.SearchResultUrlExtraction
{
    // An ISearchResultUrlExtractor which targets Google search, by default google.com.au. Only
    // genuine search results are returned: not ads.
    public class GoogleExtractor : IExtractor
    {
        const string DEFAULT_GOOGLE = "https://www.google.com.au";
        
        // If this HTML fragment occurs, we've been caught out.
        const string RECAPTCHA_INDICATOR = "<div id=\"recaptcha\" class=\"g-recaptcha\"";
        
        // After this HTML fragment commences the URL of the next page, which ends with '"'.
        const string NEXT_PAGE_INDICATOR = "<a class=\"pn\" href=\"";

        // After this HTML fragment commences the URL of a search result, which ends with '"'.
        const string SEARCH_RESULT_INDICATOR = "<h3 class=\"r\"><a href=\"";

        // The URL of the Google targeted by this GoogleExtractor (sans trailing slash)
        string googleBaseUrl;

        IDownloader downloader;

        public GoogleExtractor(string google_base_url, IDownloader downloader) {
            this.googleBaseUrl = google_base_url;
            this.downloader = downloader;
        }

        public GoogleExtractor(IDownloader downloader) : this(DEFAULT_GOOGLE, downloader) { }

        public IEnumerable<string> Extract(string search_terms)
        {
            string page = null;
            int processed_results = 0;
            bool has_next_page_link = true;

            while (true) {
                if (!has_next_page_link)
                    // We're out of "next page" links.
                    yield break;

                try {
                    page = downloader.Download(ConstructNextPageUrl(search_terms, processed_results));
                } catch (DownloadException ex) {
                    if (ex.InnerException is WebException webex
                            && (webex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.ServiceUnavailable)
                        throw new ExtractionException("Google has returned an 'unavailable' status. This can happen when too many"
                            + " searches have been performed within a short time. Please try again later.");
                    throw new ExtractionException(ex.Message, ex);
                }

                has_next_page_link = FindHref(page, NEXT_PAGE_INDICATOR, 0, out int _) != null;
                if (!has_next_page_link && page.Contains(RECAPTCHA_INDICATOR))
                    throw new ExtractionException("Google has sent a reCAPTCHA challenge. This can happen when too many searches have"
                        + " been performed within a short time. Please try again later.");

                // Comb through the search results.
                for (int start_idx = 0; start_idx >= 0;) {
                    string to_yield = FindHref(page, SEARCH_RESULT_INDICATOR, start_idx, out start_idx);
                    if (to_yield != null) {
                        processed_results++;
                        yield return to_yield;
                    }
                }
            }
        }

        // Return the HREF attribute value which follows the given indicator fragment and is
        // terminated with '"', starting from the given index. The 'out' parameter will be set to
        // the index of the terminator, i.e. the character after the end of the HREF. If the
        // indicator cannot be found, return null and set terminator_idx to -1.
        static string FindHref(string page, string indicator, int start_idx, out int terminator_idx)
        {
            int found_idx = page.IndexOf(indicator, start_idx);
            if (found_idx < 0) {
                terminator_idx = -1;
                return null;
            }
            found_idx += indicator.Length;

            terminator_idx = page.IndexOf('"', found_idx);
            if (terminator_idx < 0)
                throw new ExtractionException("Terminating '\"' missing after HREF. There might be a problem with Google.");
            if (terminator_idx == found_idx)
                throw new ExtractionException("Missing HREF. There might be a problem with Google.");

            return HttpUtility.HtmlDecode(page.Substring(found_idx, terminator_idx - found_idx));
        }

        // The "next" links on each page don't work properly without cookies so this method does the
        // pagination manually.
        string ConstructNextPageUrl(string search_terms, int skip) => $"{googleBaseUrl}/search?q={HttpUtility.UrlEncode(search_terms)}&start={skip}";
    }
}
