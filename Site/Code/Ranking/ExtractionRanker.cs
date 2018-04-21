using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scraper.Code.SearchResultUrlExtraction;

namespace Scraper.Code.Ranking
{
    // An IRanker which consumes the (first 100) results of an IExtractor
    public class ExtractionRanker : IRanker
    {
        // Up to MAX_EXTRACTION results will be processed.
        const int MAX_EXTRACTION = 100;
        
        IExtractor extractor;

        public ExtractionRanker(IExtractor extractor)
        {
            this.extractor = extractor;
        }
        
        // See the comments in IRanker for information about this method.
        public string Rank(string search_terms, string target_site)
        {
            int index = 1;
            List<int> found_indices = new List<int>();
            foreach (string url in extractor.Extract(search_terms).Take(MAX_EXTRACTION)) {
                if (IsMatch(url, target_site))
                    found_indices.Add(index);
                index++;
            }
            return found_indices.Count == 0 ? "0" : string.Join(' ', found_indices);
        }

        // Return whether the hostname part of the given url either is or ends with target_site.
        static bool IsMatch(string url, string target_site)
        {
            Uri parsed;
            try {
                parsed = new Uri(url);
            } catch (UriFormatException) {
                return false;
            }
            if (parsed.HostNameType != UriHostNameType.Dns)
                return false;

            string host_lc = parsed.Host.ToLowerInvariant(), target_lc = target_site.ToLowerInvariant();
            return host_lc == target_lc || host_lc.EndsWith("." + target_lc);
        }
    }
}
